using Microsoft.AspNetCore.Mvc;
using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;
using StudentInfoSystemApp.Application.Interfaces;

namespace StudentInfoSystemApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _attendanceService.GetAllAsync());
        }
        [HttpGet("id")]
        public async Task<IActionResult> Get(int? id)
        {
            return Ok(await _attendanceService.GetByIdAsync(id));
        }
        [HttpPost("")]
        public async Task<IActionResult>Create([FromBody]AttendanceCreateDTO attendanceCreateDTO)
        {
            return Ok(await _attendanceService.CreateAsync(attendanceCreateDTO));
        }
    }
}
