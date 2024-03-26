using ibanking_server.Dtos;
using ShareDtos;

namespace ibanking_server.SyncDataService
{
    public interface ITutionClient
    {
        public Task<ApiResponse> SendToTution(TransactionSender sender);
    }
}
