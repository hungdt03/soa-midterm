using authentication_service.Data;
using authentication_service.Dtos;
using authentication_service.Exceptions;
using authentication_service.Models;
using authentication_service.Utils;
using Microsoft.EntityFrameworkCore;

namespace authentication_service.Services
{
    public class AuthService : IAuthService
    {
        private readonly AccountDbContext _dbContext;
        private readonly PasswordEncoder passwordEncoder;
        private readonly JwtTokenUtil jwtTokenUtil;

        public AuthService(AccountDbContext dbContext, PasswordEncoder passwordEncoder, JwtTokenUtil jwtTokenUtil)
        {
            this._dbContext = dbContext;
            this.passwordEncoder = passwordEncoder;
            this.jwtTokenUtil = jwtTokenUtil;
        }
        public async Task<TokenResponse> Login(LoginRequest request)
        {
            string encodedPassword = passwordEncoder.Encode(request.Password);
            Account existedUser = await _dbContext.Accounts
                .Where(u => u.Email.Equals(request.Email) && u.Password.Equals(encodedPassword))
                .FirstOrDefaultAsync() ?? throw new BadCredentialsException("Invalid email or password");

            DateTime expireTime = DateTime.Now.AddHours(2);
            string accessToken = jwtTokenUtil.GenerateToken(existedUser, expireTime);

            return new TokenResponse(true, accessToken, expireTime, existedUser);
        }

        public async Task<ApiResponse> GetPrincipal(string email)
        {
            Account existedUser = await _dbContext.Accounts
                .Where(u => u.Email.Equals(email))
                .FirstOrDefaultAsync() ?? throw new BadCredentialsException("Unauthorized");

            return new ApiResponse(true, "Get principal successful", existedUser);
        }

        public async Task<ApiResponse> Registry(RegistryRequest request)
        {
            if (_dbContext == null)
            {
                throw new ConflictException("DB null");
            }

         
            Account? existedUser = await _dbContext.Accounts
                .Where(u => u.Email.Equals(request.Email) && u.PhoneNumber.Equals(request.PhoneNumber))
                .FirstOrDefaultAsync();

            if (existedUser != null)
                throw new ConflictException("Email or phone number is already used");

            string password = passwordEncoder.Encode(request.Password);
            Account user = new Account();
            user.Name = request.FullName;
            user.UserName = request.Username;
            user.PhoneNumber = request.PhoneNumber;
            user.Email = request.Email;
            user.Password = password;
            user.IsTrading = false;
            user.Balance = 100000000;

            await _dbContext.Accounts.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return new ApiResponse(true, "Created", "");
        }
    }
}
