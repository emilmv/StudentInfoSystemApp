using Microsoft.AspNetCore.Mvc;
using StudentInfoSystemApp.Application.DTOs.AuthDTOs;
using StudentInfoSystemApp.Application.Interfaces;

namespace StudentInfoSystemApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {

            return Ok(await _authService.RegisterAsync(registerDTO));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            return Ok(await _authService.LoginAsync(loginDTO));
        }

        [HttpGet("users")]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] string searchInput = "")
        {
            return Ok(await _authService.GetUsersAsync());
        }
    }
}
