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

        [Authorize]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost]
        public async Task<IActionResult> TransactionPaymentTutiton([FromBody] TransactionRequest request)
        {
            var token = Request.Headers["Authorization"].ToString();
            string email = User.Claims.First(u => u.Type.Equals(ClaimTypes.Email))?.Value ?? "";
            return Ok(await transactionService.TransactionPaymentTutiton(request, email));
        }

        [Authorize]
        [HttpGet("send-otp/again/{transactionId}")]
        [Authorize]
        public async Task<IActionResult> SendCodeAgain([FromRoute] int transactionId)
        {
            return Ok(await transactionService.SendCodeAgain(transactionId));
        }

        [Authorize]
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelTransaction([FromRoute] int id)
        {
            return Ok(await transactionService.CancelTransaction(id));
        }

        [Authorize]
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPRequest request)
        {
            var token = Request.Headers["Authorization"].ToString();
            string email = User.Claims.First(u => u.Type.Equals(ClaimTypes.Email))?.Value ?? "";
            return Ok(await transactionService.VerifyOTP(request, email));
        }

        [Authorize]
        [HttpGet("{accountId}")]
        [Authorize]
        public async Task<IActionResult> GetTransactionsByUserId([FromRoute] int accountId)
        {
            return Ok(await transactionService.FindAllByUserId(accountId));
        }
    }
}
