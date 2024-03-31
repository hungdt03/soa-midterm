using ibanking_server.Dtos;
using ShareDtos;

namespace ibanking_server.SyncDataService
{
    public interface ITutionClient
    {
        public Task<HttpResponseMessage> SendToTution(TransactionSender sender);
    }
}
