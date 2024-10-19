using Microsoft.AspNetCore.Mvc;
using StudentInfoSystemApp.Application.DTOs.CourseDTOs;
using StudentInfoSystemApp.Application.DTOs.DepartmentDTOs;
using StudentInfoSystemApp.Application.Services.Implementations;
using StudentInfoSystemApp.Application.Services.Interfaces;

namespace StudentInfoSystemApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] string searchInput = "", [FromQuery] int pageSize = 3)
        {
            return Ok(await _departmentService.GetAllAsync(page, searchInput, pageSize));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            return Ok(await _departmentService.GetByIdAsync(id));
        }
        [HttpPost("")]
        public async Task<IActionResult> Create([FromForm] DepartmentCreateDTO departmentCreateDTO)
        {
            return Ok(await _departmentService.CreateAsync(departmentCreateDTO));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int? id)
        {
            return Ok(await _departmentService.DeleteAsync(id));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] string departmentName)
        {
            return Ok(await _departmentService.UpdateAsync(id, departmentName));
        }
    }
}
