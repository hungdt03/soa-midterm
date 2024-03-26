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
        //private readonly IConfiguration _configuration;

        public TutionClient(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("tution-service");
            //_configuration = configuration;
        }

        public async Task<ApiResponse> SendToTution(TransactionSender sender)
        {
            var httpContent = new StringContent(
              JsonConvert.SerializeObject(sender),
              Encoding.UTF8,
              "application/json"
          );

            var response = await _httpClient.PostAsync("/tutions/payment/callback", httpContent);
            string responseData = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseData)!;
                return apiResponse!;
            }
            else
            {
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
    }
}
