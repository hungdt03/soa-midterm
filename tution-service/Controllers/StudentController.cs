using ibanking_server.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Steeltoe.Common.Discovery;
using Steeltoe.Discovery;
using tution_service.Dtos;
using tution_service.Services;

namespace tution_service.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService studentService;
        

        public StudentController(IStudentService studentService)
        {
            this.studentService = studentService;
            
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateStudent([FromBody] StudentDTO studentDTO)
        {
            return Ok(await studentService.CreateStudent(studentDTO));
        }

        [HttpGet]
        public async Task<IActionResult> FindAll()
        {
            return Ok(await studentService.FindAll());
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> FindById([FromRoute] int id)
        {
            return Ok(await studentService.FindById(id));
        }

        [HttpGet("code/{studentCode}")]
        public async Task<IActionResult> FindByStudentCode([FromRoute] string studentCode)
        {
            return Ok(await studentService.FindByStudentCode(studentCode));
        }

       
    }
}
