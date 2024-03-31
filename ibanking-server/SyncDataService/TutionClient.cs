using ibanking_server.Data;
using ibanking_server.Dtos;
using ibanking_server.Exceptions;
using Newtonsoft.Json;
using ShareDtos;
using System.Net;
using System.Text;

namespace ibanking_server.SyncDataService
{
    public class TutionClient : ITutionClient
    {
        private readonly HttpClient _httpClient;

        public TutionClient(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("tution-service");
        }

        public async Task<HttpResponseMessage> SendToTution(TransactionSender sender)
        {
            var httpContent = new StringContent(
              JsonConvert.SerializeObject(sender),
              Encoding.UTF8,
              "application/json"
          );

            var response = await _httpClient.PostAsync("/api/tutions/payment/callback", httpContent);
            return response;
          
        }
    }
}
