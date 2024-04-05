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

        public async Task<ApiResponse> TransactionPaymentTutiton(TransactionRequest transactionRequest, string email)
        {
            Account account = await dbContext.Accounts.SingleOrDefaultAsync(u => u.Email.Equals(email))
                ?? throw new BadCredentialException("Unauthorized");
            if (account.Balance < transactionRequest.Amount)
                throw new ConflictException("Số dư khả dụng của bạn không đủ!");


            string otpCode = await otpUtils.GenerateOTP();
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
            transaction.OTPs.Add(otp);
            otp.Transaction = transaction;

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

        private async Task UpdateTransaction(int accountId)
        {
            List<Transaction> transactions = await dbContext.Transactions.Where(t => t.AccountId == accountId && t.TransactionStatus.Equals(TransactionStatus.IN_PROGRESS)).ToListAsync();
            
            bool isHandle = false;
            foreach (Transaction transaction in transactions)
            {
                TimeSpan spaceTime = DateTime.Now - transaction.StartTransactionTime;
                if (spaceTime.TotalMinutes > 5)
                {
                    transaction.TransactionStatus = TransactionStatus.CANCELLED;
                } else
                {
                    isHandle = true;
                }
            }

            await dbContext.SaveChangesAsync();
            
            if (isHandle)
                throw new ConflictException("Có một giao dịch khác đang được xử lí!");
        }


        public async Task<ApiResponse> VerifyOTP(VerifyOTPRequest request, string email)
        {
            Account account = await dbContext.Accounts.SingleOrDefaultAsync(u => u.Email.Equals(email))
                ?? throw new BadCredentialException("Unauthorized");

            Transaction transaction = await dbContext.Transactions
                .Include(t => t.Account)
                .SingleOrDefaultAsync(t => t.Id == request.TransactionId) ??
                    throw new NotFoundException($"Không tìm thấy giao dịch = {request.TransactionId}");
            if(transaction.TutionId == request.TutionId && transaction.TransactionStatus.Equals(TransactionStatus.IN_PROGRESS)) {
                throw new ConflictException("Phần học phí này đang được xử lí bởi một giao dịch khác");
            }

            if (transaction.TransactionStatus.Equals(TransactionStatus.CANCELLED))
                throw new ConflictException($"Giao dịch này đã bị hủy bỏ!");

            if (transaction.TransactionStatus.Equals(TransactionStatus.COMPLETED))
                throw new ConflictException($"Giao dịch này đã hoàn thành vào lúc {transaction.EndTransactionTime.ToString("HH:mm:ss dd/MM/yyyy")}");


            await UpdateTransaction(account.Id); 
            await otpUtils.VerifyOTP(request.Otp, request.TransactionId);
            
            transaction.TransactionStatus = TransactionStatus.IN_PROGRESS;
            await dbContext.SaveChangesAsync();
            
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
                return new ApiResponse(true, "Giao dịch thành công", MapTransaction(transaction));
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
                TransactionStatus = transaction.TransactionStatus.ToString(),
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

        public async Task<ApiResponse> SendCodeAgain(int transactionId)
        {
            Transaction? transaction = await dbContext.Transactions
                .Include(t => t.OTPs)
                .Include(t => t.Account)
                .SingleOrDefaultAsync(t => t.Id == transactionId)
                    ?? throw new NotFoundException("Không tìm thấy giao dịch");

            TimeSpan spaceTime = DateTime.Now - transaction.StartTransactionTime;
            if (spaceTime.TotalMinutes > 5)
            {
                transaction.TransactionStatus = TransactionStatus.CANCELLED;
                await dbContext.SaveChangesAsync();
                throw new ConflictException("Phiên giao dịch đã quá hạn");
            }

            if(transaction.OTPs.Count > 0)
            {
                transaction.OTPs[transaction.OTPs.Count - 1].OTPStatus = OTPStatus.INVALID;
            }

            string otpCode = await otpUtils.GenerateOTP();
            OTP otp = new OTP();
            otp.OTPCode = otpCode;
            otp.CreatedAt = DateTime.Now;
            otp.ExpiredAt = DateTime.Now.AddMinutes(5);
            otp.Transaction = transaction;

            await dbContext.OTPs.AddAsync(otp);
            await dbContext.SaveChangesAsync();

            string to = transaction.Account.Email;
            string subject = "Mã OTP Xác Nhận";
            string body = $@"
                Chào bạn {transaction.Account.Name},
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

            return new ApiResponse(true, "Gửi mã thành công", null);
        }
    }
}
