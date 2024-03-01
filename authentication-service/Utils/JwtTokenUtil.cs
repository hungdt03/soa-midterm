using authentication_service.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace authentication_service.Utils
{
    public class JwtTokenUtil
    {
        private IConfiguration _config;
        public JwtTokenUtil(IConfiguration configuration)
        {
            _config = configuration;
        }
        public string GenerateToken(Account account, DateTime expireTime)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKeyBytes = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"] ?? ""));
            var credentials = new SigningCredentials(secretKeyBytes, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, account.Name),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.MobilePhone, account.PhoneNumber),
            };

            var token = new JwtSecurityToken(
              _config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: expireTime,
              signingCredentials: credentials
            );

            return jwtTokenHandler.WriteToken(token);
        }
    }
}
