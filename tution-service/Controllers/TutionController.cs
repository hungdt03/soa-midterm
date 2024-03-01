using ibanking_server.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tution_service.Dtos;
using tution_service.Services;

namespace tution_service.Controllers
{
    [Route("api/tutions")]
    [ApiController]
    public class TutionController : ControllerBase
    {
        private readonly ITutionService tutionService;

        public TutionController(ITutionService tutionService)
        {
            this.tutionService = tutionService;
        }

        [HttpGet("{studentCode}")]
        public async Task<IActionResult> getByStudentCode([FromRoute] string studentCode)
        {
            return Ok(await tutionService.FindAllByStudentCode(studentCode));
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateTution([FromBody] TutionRequest tutionRequest)
        {
            return Ok(await tutionService.CreateTution(tutionRequest));
        }

        [HttpPost("payment")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> PaymentTution([FromBody] PaymentRequest paymentRequest)
        {
            var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                return Ok(await tutionService.PaymentTution(paymentRequest, token));
            }

            return Unauthorized();
            
        }
    }
}
