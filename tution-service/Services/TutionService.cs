using Microsoft.EntityFrameworkCore;
using ShareDtos;
using System;
using tution_service.Data;
using tution_service.Dtos;
using tution_service.Dtos.ClientDtos;
using tution_service.Enums;
using tution_service.Exceptions;
using tution_service.Models;
using tution_service.SyncDataServices;

namespace tution_service.Services
{
    public class TutionService : ITutionService
    {
        private readonly TutionDbContext _dbContext;
        private readonly ITransactionClient transactionClient;

        public TutionService(TutionDbContext dbContext, ITransactionClient transactionClient)
        {
            this._dbContext = dbContext;
            this.transactionClient = transactionClient;
        }

        public async Task<ApiResponse> CreateTution(TutionRequest tutionRequest)
        {
            Student student = await _dbContext.Students
                .SingleOrDefaultAsync(s => s.StudentCode.Equals(tutionRequest.StudentCode))
                ?? throw new NotFoundException("Mã số sinh viên không dúng");

            Tution? checkedTution = await _dbContext.Tutions
                .Include(t => t.Student)
                .SingleOrDefaultAsync(t => t.TutionCode.Equals(tutionRequest.TutionCode));

            if(checkedTution != null && checkedTution!.Student.StudentCode.Equals(tutionRequest.StudentCode))
                throw new ConflictException($"Mã học phí `{checkedTution.TutionCode}` của {checkedTution.Student.FullName} đã tồn tại");


            Tution tution = new Tution();
            tution.Status = TutionStatus.UNPAID;
            tution.Amount = tutionRequest.Amount;
            tution.Student = student;
            tution.TutionCode = tutionRequest.TutionCode;
            tution.StartAt = tutionRequest.StartAt;
            tution.EndAt = tutionRequest.EndAt;
            tution.Description = tutionRequest.Description;

            await _dbContext.Tutions.AddAsync(tution);
            await _dbContext.SaveChangesAsync();

            return new ApiResponse(true, "Học phí đã được tạo", MapTution(tution));
        }

        private object MapTution(Tution tution)
        {
            if (tution == null) return null;
            return new
            {
                Id = tution.Id,
                Amount = tution.Amount,
                TutionCode = tution.TutionCode,
                Description = tution.Description,
                StudentName = tution.Student.FullName,
                StudentCode = tution.Student.StudentCode,
                ExpireTime = tution.EndAt
            };
        }

        public async Task<ApiResponse> FindAllByStudentCode(string studentCode)
        {
            Student student = await _dbContext.Students
               .SingleOrDefaultAsync(s => s.StudentCode.Equals(studentCode))
               ?? throw new NotFoundException("Mã số sinh viên không dúng");

            List<Tution> tutions = await _dbContext.Tutions
               .Where(t => t.Student.StudentCode.Equals(studentCode)).ToListAsync();

            return new ApiResponse(true, "Lấy dữ liệu học phí thành công", tutions);
        }

        public async Task<ApiResponse> PaymentTution(PaymentRequest paymentRequest, string token)
        {
            Student student = await _dbContext.Students
               .SingleOrDefaultAsync(s => s.Id == paymentRequest.StudentId)
               ?? throw new NotFoundException("Sinh viên không tồn tại");

            Tution? checkedTution = await _dbContext.Tutions
                .SingleOrDefaultAsync(t => t.Id.Equals(paymentRequest.TutionId) && t.StudentId == paymentRequest.StudentId)
                    ?? throw new NotFoundException($"Dữ liệu học phí của sinh viên này không tồn tại");

            if (checkedTution.Status.Equals(TutionStatus.PAID))
                throw new ConflictException("Phần học phí này đã được thanh toán");

            TransactionRequest request = new TransactionRequest();
            request.Amount = checkedTution.Amount;
            request.Content = paymentRequest.Content;
            request.TutionId = paymentRequest.TutionId;

            // Gọi tới banking-service
            ApiResponse response = await transactionClient.TransactionPaymentTutiton(request, token);
            int transactionCode = (int)(long)response.Data;
            checkedTution.TransactionId = transactionCode;
            await _dbContext.SaveChangesAsync();

            return response;
        }

        public async Task<ApiResponse> Callback(TransactionSender callbackReq)
        {

            if(callbackReq.IsSuccess)
            {
                Tution? checkedTution = await _dbContext.Tutions
                .SingleOrDefaultAsync(t => t.Id.Equals(callbackReq.TutionId))
                    ?? throw new NotFoundException($"Không tìm thấy học phí có mã = {callbackReq.TutionId}");

                if (checkedTution.Status.Equals(TutionStatus.PAID))
                    throw new ConflictException("Phần học phí này đã được thanh toán");

                checkedTution.Status = TutionStatus.PAID;

                await _dbContext.SaveChangesAsync();
                return new ApiResponse(true, "Cập nhật trạng thái học phí thành công", null);
            }

            throw new ConflictException("Giao dịch thất bại");
            
        }
    }
}
