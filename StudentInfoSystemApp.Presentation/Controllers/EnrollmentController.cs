﻿using Microsoft.AspNetCore.Mvc;
using StudentInfoSystemApp.Application.Implementations;
using StudentInfoSystemApp.Application.Interfaces;

namespace StudentInfoSystemApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentSealService;

        public EnrollmentController(IEnrollmentService enrollmentSealService)
        {
            _enrollmentSealService = enrollmentSealService;
        }
        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _enrollmentSealService.GetAllAsync());
        }
        [HttpGet("id")]
        public async Task<IActionResult> Get(int? id)
        {
            return Ok(await _enrollmentSealService.GetByIdAsync(id));
        }
    }
}
