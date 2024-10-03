using Microsoft.AspNetCore.Mvc;
using StudentInfoSystemApp.Application.DTOs.ProgramDTOs;
using StudentInfoSystemApp.Application.Implementations;
using StudentInfoSystemApp.Application.Interfaces;

namespace StudentInfoSystemApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramController : ControllerBase
    {
        private readonly IProgramService _programService;

        public ProgramController(IProgramService programService)
        {
            _programService = programService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] string searchInput = "")
        {
            return Ok(await _programService.GetAllAsync(page, searchInput));
        }
        [HttpGet("id")]
        public async Task<IActionResult> Get(int? id)
        {
            return Ok(await _programService.GetByIdAsync(id));
        }
        [HttpPost("")]
        public async Task<IActionResult> Create([FromForm] ProgramCreateDTO programCreateDTO)
        {
            return Ok(await _programService.CreateAsync(programCreateDTO));
        }
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteAsync(int? id)
        {
            return Ok(await _programService.DeleteAsync(id));
        }
    }
}
