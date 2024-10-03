using Microsoft.AspNetCore.Mvc;
using StudentInfoSystemApp.Application.DTOs.StudentDTOs;
using StudentInfoSystemApp.Application.Implementations;
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
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] string searchInput = "")
        {
            return Ok(await _studentService.GetAllAsync(page, searchInput));
        }
        [HttpGet("id")]
        public async Task<IActionResult> Get(int? id)
        {
            return Ok(await _studentService.GetByIdAsync(id));
        }
        [HttpPost("")]
        public async Task<IActionResult> Create([FromForm] StudentCreateDTO studentCreateDTO)
        {
            return Ok(await _studentService.CreateAsync(studentCreateDTO));
        }
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteAsync(int? id)
        {
            return Ok(await _studentService.DeleteAsync(id));
        }
    }
}
