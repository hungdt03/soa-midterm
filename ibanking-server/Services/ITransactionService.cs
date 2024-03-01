using ibanking_server.Dtos;

namespace ibanking_server.Services
{
    public interface ITransactionService
    {
        public Task<ApiResponse> TransactionPaymentTutiton(TransactionRequest transactionRequest, string email);
        public Task<ApiResponse> VerifyOTP(VerifyOTPRequest request);
        //public Task<ApiResponse> FindAllByUserId(int id);
    }
}
