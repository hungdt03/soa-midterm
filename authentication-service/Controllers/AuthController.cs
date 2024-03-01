﻿
using authentication_service.Dtos;
using authentication_service.Exceptions;
using authentication_service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        //[HttpGet]
        //public async Task<IActionResult> GetPrincipal()
        //{
        //    string email = HttpContext.User.Claims.FirstOrDefault(u => u.Type.Equals(ClaimTypes.Email))?.Value ?? "";
        //    return Ok(await userService.GetPrincipal(email));
        //}

        [AllowAnonymous]
        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            return Ok(await userService.Login(req));
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Signup([FromBody] RegistryRequest req)
        {
            return Ok(await userService.Registry(req));
        }
    }
}
