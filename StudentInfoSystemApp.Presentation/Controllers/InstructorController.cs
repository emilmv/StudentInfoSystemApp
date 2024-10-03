using Microsoft.AspNetCore.Mvc;
using StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs;
using StudentInfoSystemApp.Application.DTOs.InstructorDTOs;
using StudentInfoSystemApp.Application.Implementations;
using StudentInfoSystemApp.Application.Interfaces;

namespace StudentInfoSystemApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        private readonly IInstructorService _instructorService;

        public InstructorController(IInstructorService instructorService)
        {
            _instructorService = instructorService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] string searchInput = "")
        {
            return Ok(await _instructorService.GetAllAsync(page, searchInput));
        }
        [HttpGet("id")]
        public async Task<IActionResult> Get(int? id)
        {
            return Ok(await _instructorService.GetByIdAsync(id));
        }
        [HttpPost("")]
        public async Task<IActionResult> Create([FromForm] InstructorCreateDTO instructorCreateDTO)
        {
            return Ok(await _instructorService.CreateAsync(instructorCreateDTO));
        }
    }
}
