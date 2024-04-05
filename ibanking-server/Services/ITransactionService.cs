using ibanking_server.Dtos;

namespace ibanking_server.Services
{
    public interface ITransactionService
    {
        public Task<ApiResponse> TransactionPaymentTutiton(TransactionRequest transactionRequest, string email);
        public Task<ApiResponse> VerifyOTP(VerifyOTPRequest request, string email);
        public Task<ApiResponse> CancelTransaction(int id);
        public Task<ApiResponse> FindAllByUserId(int id);
        public Task<ApiResponse> SendCodeAgain(int transactionId);
    }
}
