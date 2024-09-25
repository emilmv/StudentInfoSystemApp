using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Get()
        {
            return Ok(await _programService.GetAllAsync());
        }
        [HttpGet("id")]
        public async Task<IActionResult> Get(int? id)
        {
            return Ok(await _programService.GetByIdAsync(id));
        }
    }
}
