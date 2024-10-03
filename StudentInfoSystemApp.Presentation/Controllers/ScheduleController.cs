using Microsoft.AspNetCore.Mvc;
using StudentInfoSystemApp.Application.DTOs.ScheduleDTOs;
using StudentInfoSystemApp.Application.Implementations;
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
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] string searchInput = "")
        {
            return Ok(await _scheduleService.GetAllAsync(page, searchInput));
        }
        [HttpGet("id")]
        public async Task<IActionResult> Get(int? id)
        {
            return Ok(await _scheduleService.GetByIdAsync(id));
        }
        [HttpPost("")]
        public async Task<IActionResult> Create([FromForm] ScheduleCreateDTO scheduleCreateDTO)
        {
            return Ok(await _scheduleService.CreateAsync(scheduleCreateDTO));
        }
    }
}
