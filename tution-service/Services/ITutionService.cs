using ShareDtos;
using tution_service.Dtos;

namespace tution_service.Services
{
    public interface ITutionService
    {
        Task<ApiResponse> FindAllByStudentCode(string studentCode);
        Task<ApiResponse> CreateTution(TutionRequest tutionRequest);
        Task<ApiResponse> PaymentTution(PaymentRequest paymentRequest, string token);
        Task<ApiResponse> Callback(TransactionSender callbackReq);

    }
}
