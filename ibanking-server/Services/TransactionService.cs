using ibanking_server.Data;
using ibanking_server.Dtos;
using ibanking_server.Enums;
using ibanking_server.Exceptions;
using ibanking_server.Models;
using ibanking_server.SyncDataService;
using ibanking_server.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using ShareDtos;
using System.Net;

namespace ibanking_server.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly OTPUtils otpUtils;
        private readonly EmailUtils emailUtils;
        private readonly BankingDbContext dbContext;
        private readonly ITutionClient tutionClient;

        public TransactionService(OTPUtils otpUtils, BankingDbContext dbContext, EmailUtils emailUtils, ITutionClient tutionClient)
        {
            this.otpUtils = otpUtils;
            this.emailUtils = emailUtils;
            this.dbContext = dbContext;
            this.tutionClient = tutionClient;
        }

        //private async Task RemoveOldTransactionSession(int accountID)
        //{
        //    OTP? lastOTP = await dbContext.OTPs
        //        .Where(o => o.AccountId == accountID)
        //        .OrderBy(t => t.Id)
        //        .LastOrDefaultAsync();

        //    if (lastOTP != null)
        //    {
        //        dbContext.OTPs.Remove(lastOTP);
        //        Transaction? oldTransaction = await dbContext.Transactions
        //            .OrderBy(t => t.Id)
        //            .LastOrDefaultAsync();

        //        if (oldTransaction != null && oldTransaction.TransactionStatus.Equals(TransactionStatus.UNCOMPLETED))
        //            dbContext.Transactions.Remove(oldTransaction);
        //        await dbContext.SaveChangesAsync();
        //    }

        //}

        public async Task<ApiResponse> TransactionPaymentTutiton(TransactionRequest transactionRequest, string email)
        {
            Account account = await dbContext.Accounts.SingleOrDefaultAsync(u => u.Email.Equals(email))
                ?? throw new BadCredentialException("Unauthorized");
            if (account.Balance < transactionRequest.Amount)
                throw new ConflictException("Your balance is less than the amount of tution");


            string otpCode = otpUtils.GenerateOTP();
            OTP otp = new OTP();
            otp.OTPCode = otpCode;
            otp.CreatedAt = DateTime.Now;
            otp.ExpiredAt = DateTime.Now.AddMinutes(5);
            
            Transaction transaction = new Transaction();
            transaction.Account = account;
            transaction.TransactionStatus = TransactionStatus.UNCOMPLETED;
            transaction.Amount = transactionRequest.Amount;
            transaction.StartTransactionTime = DateTime.Now;
            transaction.Content = transactionRequest.Content;
            transaction.TransactionType = TransactionType.TutionPayment;
            transaction.TutionId = transactionRequest.TutionId;
            transaction.OTP = otp;

            EntityEntry<Transaction> tr = await dbContext.Transactions.AddAsync(transaction);
            
            await dbContext.SaveChangesAsync();

            string to = account.Email;
            string subject = "Mã OTP Xác Nhận";
            string body = $@"
                Chào bạn {account.Name},
                Dưới đây là mã OTP (One-Time Password) của bạn để xác nhận giao dịch:
                Mã OTP: {otpCode}
                Vui lòng nhập mã này vào trang thanh toán để hoàn tất giao dịch.
                Lưu ý rằng mã OTP chỉ có hiệu lực trong 5 phút và chỉ sử dụng được một lần. Để bảo mật tài khoản của bạn, xin đừng chia sẻ mã này với người khác.
                Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.
                Trân trọng,
                IBanking
            ";

            if (!emailUtils.SendMail(to, body, subject))
                throw new Exception("Error when sending email");

            return new ApiResponse(true, "Sucessfully to send OTP", (int)tr.Entity.Id);
        }


        public async Task<ApiResponse> VerifyOTP(VerifyOTPRequest request, string email)
        {
            Account account = await dbContext.Accounts.SingleOrDefaultAsync(u => u.Email.Equals(email))
                ?? throw new BadCredentialException("Unauthorized");

            bool check = await dbContext.Transactions.AnyAsync(t => t.AccountId == account.Id && t.TransactionStatus.Equals(TransactionStatus.IN_PROGRESS));
            if (check)
                throw new ConflictException("Đang có một giao dịch khác đang được xử lí!");

            Transaction transaction = await dbContext.Transactions
                .Include(t => t.Account)
                .SingleOrDefaultAsync(t => t.Id == request.TransactionId) ??
                    throw new NotFoundException($"Không tìm thấy giao dịch = {request.TransactionId}");

            if (transaction.TransactionStatus.Equals(TransactionStatus.CANCELLED))
                throw new ConflictException($"Giao dịch này đã bị hủy bỏ!");

            if (transaction.TransactionStatus.Equals(TransactionStatus.COMPLETED))
                throw new ConflictException($"Giao dịch này đã hoàn thành vào lúc {transaction.EndTransactionTime.ToString("HH:mm:ss dd/MM/yyyy")}");
            
            transaction.TransactionStatus = TransactionStatus.IN_PROGRESS;
            await dbContext.SaveChangesAsync();

            await otpUtils.VerifyOTP(request.Otp, transaction.Id);

            Account user = transaction.Account;
            TransactionSender transactionEvent = new TransactionSender
            {
                TransactionId = transaction.Id,
                AccountId = user.Id,
                AccountName = user.Name,
                IsSuccess = true,
                TutionId = transaction.TutionId,
                Amount = transaction.Amount,
                Content = transaction.Content,
                StartTransactionTime = transaction.StartTransactionTime,
                EndTransactionTime = transaction.EndTransactionTime,
            };

            // Gọi tới service-học phí để gạch nợ phần học phí
            HttpResponseMessage response = await tutionClient.SendToTution(transactionEvent);

            if (response.IsSuccessStatusCode)
            {

                transaction.TransactionStatus = TransactionStatus.COMPLETED;
                transaction.EndTransactionTime = DateTime.Now;

                
                user.Balance -= transaction.Amount;
                user.IsTrading = false;
                await dbContext.SaveChangesAsync();

                string to = user.Email;
                string subject = "Xác Nhận Thanh Toán Giao Dịch Thành Công";
                string body = $@"
                    Chào {user.Name},
                    Chúng tôi xin thông báo rằng giao dịch thanh toán của bạn đã được thực hiện thành công. Dưới đây là chi tiết về phiên giao dịch:
                    Mã giao dịch: {transaction.Id}
                    Loại giao dịch: {transaction.TransactionType.ToString()}
                    Số Tiền Thanh Toán: {transaction.Amount} VND
                    Thời gian bắt đầu phiên giao dịch: {transaction.StartTransactionTime.ToString("hh:mm:ss dd/MM/yyyy")}
                    Thời gian kết thúc phiên giao dịch: {transaction.EndTransactionTime.ToString("hh:mm:ss dd/MM/yyyy")}
                    Phương Thức Thanh Toán: Internet Banking
                    Số Dư Khả Dụng Hiện Tại: {user.Balance} VND
                    Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi. Nếu có bất kỳ câu hỏi hoặc thắc mắc nào, vui lòng liên hệ chúng tôi ngay lập tức.
                    Trân trọng,
                    IBanking
                ";

                emailUtils.SendMail(to, body, subject);
                return new ApiResponse(true, "Transaction information", MapTransaction(transaction));
            }
            else
            {
                transaction.TransactionStatus = TransactionStatus.CANCELLED;
                await dbContext.SaveChangesAsync();
                string responseData = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseData);

                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    throw new ConflictException(apiResponse?.Message);
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new NotFoundException(apiResponse?.Message);
                }
                else
                {
                    throw new Exception($"Có lỗi: {apiResponse?.Message}");
                }
            }

            
        }

        private object MapTransaction(Transaction transaction)
        {
            if (transaction == null) return transaction!;
            return new
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                StartTransactionTime = transaction.StartTransactionTime,
                EndTransactionTime = transaction.EndTransactionTime,
                TransactionType = transaction.TransactionType.ToString(),
                AccountName = transaction.Account.Name,
                AccountId = transaction.Account.Id,
            };
        }

        public ICollection<object> MapToList(ICollection<Transaction> transactions)
        {
            return transactions.Select(MapTransaction).ToList();
        }

        public async Task<ApiResponse> FindAllByUserId(int id)
        {
            ICollection<Transaction> transactions = await dbContext.Transactions
                .Include (t => t.Account)
                .Where(t => t.AccountId.Equals(id)).ToListAsync();

            return new ApiResponse(true, "Lấy thông tin giao dịch thành công", MapToList(transactions));
        }

        public async Task<ApiResponse> CancelTransaction(int id)
        {
            Transaction? transaction = await dbContext.Transactions
                .Include(t => t.Account)
                .SingleOrDefaultAsync(t => t.Id == id)
                    ?? throw new NotFoundException("Không tìm thấy giao dịch");

            transaction.TransactionStatus = TransactionStatus.CANCELLED;
            transaction.Account.IsTrading = false;
            await dbContext.SaveChangesAsync();
            return new ApiResponse(true, "Hủy bỏ giao dịch thành công", null);
        }
    }
}
