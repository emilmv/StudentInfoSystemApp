using Microsoft.AspNetCore.Mvc;
using StudentInfoSystemApp.Application.DTOs.CourseDTOs;
using StudentInfoSystemApp.Application.Implementations;
using StudentInfoSystemApp.Application.Interfaces;

namespace StudentInfoSystemApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery]int page = 1, [FromQuery]string searchInput="")
        {
            return Ok(await _courseService.GetAllAsync(page,searchInput));
        }
        [HttpGet("id")]
        public async Task<IActionResult> Get(int? id)
        {
            return Ok(await _courseService.GetByIdAsync(id));
        }
        [HttpPost("")]
        public async Task<IActionResult> Create([FromForm] CourseCreateDTO courseCreateDTO)
        {
            return Ok(await _courseService.CreateAsync(courseCreateDTO));
        }
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteAsync(int? id)
        {
            return Ok(await _courseService.DeleteAsync(id));
        }
    }
}
