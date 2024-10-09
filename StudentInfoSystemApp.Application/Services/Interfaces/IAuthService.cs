using StudentInfoSystemApp.Application.DTOs.AuthDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;

namespace StudentInfoSystemApp.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDTO registerDTO);
        Task<string> LoginAsync(LoginDTO loginDTO);
        Task<PaginationListDTO<UserReturnDTO>> GetAllAsync(int page = 1, string searchInput = "");
        Task<bool> DeleteAsync(string userId);
        Task<string> VerifyEmailAsync(string email, string token);
        Task<string> ResendVerificationEmailAsync(string email);
        Task<string> SendPasswordResetEmailAsync(string email);
        Task<string> ResetPasswordAsync(string email, string token, ResetPasswordDTO resetPasswordDTO);
    }
}
