using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(ApplicationUser existingUser, List<string> userRoles);
    }
}
