﻿using Microsoft.AspNetCore.Mvc;
using StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs;
using StudentInfoSystemApp.Application.Services.Implementations;
using StudentInfoSystemApp.Application.Services.Interfaces;

namespace StudentInfoSystemApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentSealService)
        {
            _enrollmentService = enrollmentSealService;
        }
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] string searchInput = "", [FromQuery] int pageSize = 3)
        {
            return Ok(await _enrollmentService.GetAllAsync(page, searchInput, pageSize));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            return Ok(await _enrollmentService.GetByIdAsync(id));
        }
        [HttpPost("")]
        public async Task<IActionResult> Create([FromForm] EnrollmentCreateDTO enrollmentCreateDTO)
        {
            return Ok(await _enrollmentService.CreateAsync(enrollmentCreateDTO));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int? id)
        {
            return Ok(await _enrollmentService.DeleteAsync(id));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] EnrollmentUpdateDTO enrollmentUpdateDTO)
        {
            return Ok(await _enrollmentService.UpdateAsync(id, enrollmentUpdateDTO));
        }
    }
}
