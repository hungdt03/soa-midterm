using authentication_service.Dtos;

namespace authentication_service.Services
{
    public interface IAuthService
    {
        public Task<TokenResponse> Login(LoginRequest request);
        public Task<ApiResponse> Registry(RegistryRequest request);
        public Task<ApiResponse> GetPrincipal(string email);
    }
}
