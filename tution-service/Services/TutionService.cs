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
            Tution? checkedTution = await _dbContext.Tutions
                .Include(t => t.Student)
                .SingleOrDefaultAsync(t => t.TutionCode.Equals(tutionRequest.TutionCode) && t.StudentId == tutionRequest.StudentId);

            if (checkedTution != null)
                throw new ConflictException($"The tution's code `{checkedTution.TutionCode}` of {checkedTution.Student.FullName} have already existed");

            Student student = await _dbContext.Students
                .SingleOrDefaultAsync(s => s.Id == tutionRequest.StudentId)
                ?? throw new NotFoundException("Student not found");

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

            return new ApiResponse(true, "Tution is created", MapTution(tution));
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
            List<Tution> tutions = await _dbContext.Tutions
               .Where(t => t.Student.StudentCode.Equals(studentCode)).ToListAsync();

            return new ApiResponse(true, "Lấy dữ liệu học phí thành công", tutions);
        }

        public async Task<ApiResponse> PaymentTution(PaymentRequest paymentRequest, string token)
        {
            Tution? checkedTution = await _dbContext.Tutions
                .SingleOrDefaultAsync(t => t.Id.Equals(paymentRequest.TutionId) && t.StudentId == paymentRequest.StudentId)
                    ?? throw new NotFoundException($"Dữ liệu học phí có id= {paymentRequest.TutionId} của MSSV = {paymentRequest.StudentId} không tìm thấy");

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
