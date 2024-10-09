using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(ApplicationUser existingUser, List<string> userRoles);
    }
}
