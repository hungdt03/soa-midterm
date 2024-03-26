using System.Text.Json;
using System.Text;
using tution_service.Dtos;
using tution_service.Dtos.ClientDtos;
using System.Net;
using tution_service.Exceptions;
using Azure.Core;
using System.Net.Http.Headers;
using System.Net.Http;
using Azure;
using Newtonsoft.Json;
using tution_service.Services;
using tution_service.Models;
using tution_service.Data;
using Microsoft.EntityFrameworkCore;
using tution_service.Enums;

namespace tution_service.SyncDataServices
{
    public class TransactionClient : ITransactionClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TransactionClient(IHttpClientFactory factory, IConfiguration configuration)
        {
            this._httpClient = factory.CreateClient("banking-service");
            _configuration = configuration;
    
        }

        public async Task<ApiResponse> TransactionPaymentTutiton(TransactionRequest request, string token)
        {
            var httpContent = new StringContent(
                JsonConvert.SerializeObject(request),
                Encoding.UTF8,
                "application/json"
            );

            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsync("/transactions", httpContent);

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseData)!;
               
                return apiResponse!;
            }
            else
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseData);
                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    throw new ConflictException(apiResponse?.Message);
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new BadCredentialException(apiResponse?.Message);
                }
                else
                {
                    throw new Exception($"Unexpected response: {apiResponse?.Message}");
                }
            }
        }

        
    }
}
