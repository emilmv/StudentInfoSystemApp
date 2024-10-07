using StudentInfoSystemApp.Application.DTOs.AuthDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDTO registerDTO);
        Task<string> LoginAsync(LoginDTO loginDTO);
        Task<List<UserReturnDTO>> GetUsersAsync();
        Task<bool> DeleteAsync(string userId);
    }
}
