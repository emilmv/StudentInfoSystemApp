using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentInfoSystemApp.Application.DTOs.AuthDTOs;
using StudentInfoSystemApp.Application.Services.Interfaces;

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
        public async Task<IActionResult> Register([FromForm]RegisterDTO registerDTO)
        {
            return Ok(await _authService.RegisterAsync(registerDTO));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm]LoginDTO loginDTO)
        {
            return Ok(await _authService.LoginAsync(loginDTO));
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            return Ok(await _authService.VerifyEmailAsync(email, token));
        }

        [HttpGet("users")]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] string searchInput = "")
        {
            return Ok(await _authService.GetAllAsync(page, searchInput));
        }
        [HttpDelete("userId")]
        public async Task<IActionResult> Delete([FromBody] string userID)
        {
            return Ok(await _authService.DeleteAsync(userID));
        }

        [HttpPost("resend-verification-email")]
        public async Task<IActionResult> ResendVerificationEmail([FromBody] ResendVerificationEmailDTO resendVerificationEmailDTO)
        {
            return Ok(await _authService.ResendVerificationEmailAsync(resendVerificationEmailDTO.Email));
        }

        [HttpPost("request-reset-password")]
        public async Task<IActionResult> RequestResetPassword([FromForm] string email)
        {
            return Ok(await _authService.SendPasswordResetEmailAsync(email));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromQuery] string email, [FromQuery] string token, [FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            return Ok(await _authService.ResetPasswordAsync(email, token, resetPasswordDTO));
        }
    }
}
