using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Get()
        {
            return Ok(await _courseService.GetAllAsync());
        }
    }
}
