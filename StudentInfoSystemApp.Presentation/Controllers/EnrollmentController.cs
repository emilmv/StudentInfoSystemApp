﻿using Microsoft.AspNetCore.Mvc;
using StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs;
using StudentInfoSystemApp.Application.Interfaces;

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
        public async Task<IActionResult> Get()
        {
            return Ok(await _enrollmentService.GetAllAsync());
        }
        [HttpGet("id")]
        public async Task<IActionResult> Get(int? id)
        {
            return Ok(await _enrollmentService.GetByIdAsync(id));
        }
        [HttpPost("")]
        public async Task<IActionResult> Create([FromBody] EnrollmentCreateDTO enrollmentCreateDTO)
        {
            return Ok(await _enrollmentService.CreateAsync(enrollmentCreateDTO));
        }
    }
}
