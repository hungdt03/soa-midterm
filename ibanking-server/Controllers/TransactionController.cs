using ibanking_server.Dtos;
using ibanking_server.Exceptions;
using ibanking_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace ibanking_server.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            this.transactionService = transactionService;
        }


        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost]
        public async Task<IActionResult> TransactionPaymentTutiton([FromBody] TransactionRequest request)
        {
            var token = Request.Headers["Authorization"].ToString();
            string email = User.Claims.First(u => u.Type.Equals(ClaimTypes.Email))?.Value ?? "";
            return Ok(await transactionService.TransactionPaymentTutiton(request, email));
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPRequest request)
        {
            return Ok(await transactionService.VerifyOTP(request));
        }
    }
}
