using Microsoft.AspNetCore.Mvc;
using StudentInfoSystemApp.Application.Interfaces;

namespace StudentInfoSystemApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _scheduleService.GetAllAsync());
        }
        [HttpGet("id")]
        public async Task<IActionResult> Get(int? id)
        {
            return Ok(await _scheduleService.GetByIdAsync(id));
        }
    }
}
