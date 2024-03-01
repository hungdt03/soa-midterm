using System.Transactions;
using tution_service.Dtos;
using tution_service.Dtos.ClientDtos;

namespace tution_service.SyncDataServices
{
    public interface ITransactionClient
    {
        Task<ApiResponse> TransactionPaymentTutiton(TransactionRequest request, string token);
    }
}
