using Microsoft.AspNetCore.Mvc;
using StudentInfoSystemApp.Application.Interfaces;

namespace StudentInfoSystemApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _studentService.GetAllAsync());
        }
    }
}
