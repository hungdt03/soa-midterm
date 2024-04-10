
using authentication_service.Dtos;
using authentication_service.Exceptions;
using authentication_service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace authentication_service.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService userService;

        public AuthController(IAuthService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPrincipal()
        {
            string email = HttpContext.User.Claims.FirstOrDefault(u => u.Type.Equals(ClaimTypes.Email))?.Value ?? "";
            return Ok(await userService.GetPrincipal(email));
        }

        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            return Ok(await userService.Login(req));
        }

        [HttpPost("signup")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Signup([FromBody] RegistryRequest req)
        {
            return Ok(await userService.Registry(req));
        }
    }
}
