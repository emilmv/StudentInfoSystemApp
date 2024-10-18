using Microsoft.AspNetCore.Mvc;
using StudentInfoSystemApp.Application.DTOs.StudentDTOs;
using StudentInfoSystemApp.Application.Services.Interfaces;

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
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] string searchInput = "",[FromQuery]int pageSize=3)
        {
            return Ok(await _studentService.GetAllAsync(page, searchInput,pageSize));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            return Ok(await _studentService.GetByIdAsync(id));
        }

        [HttpPost("")]
        public async Task<IActionResult> Create([FromForm] StudentCreateDTO studentCreateDTO)
        {
            return Ok(await _studentService.CreateAsync(studentCreateDTO));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int? id)
        {
            return Ok(await _studentService.DeleteAsync(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] StudentUpdateDTO studentUpdateDTO)
        {
            return Ok(await _studentService.UpdateAsync(id, studentUpdateDTO));
        }
    }
}
