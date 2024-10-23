using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.Helpers.EntityHelpers
{
    public static class AuthHelper
    {
        public static async Task CheckUserAvailabilityAsync(UserManager<ApplicationUser> _userManager, string username, string email)
        {
            //Checking for username
            ApplicationUser existingUserWithUsername = await _userManager.FindByNameAsync(username);
            if (existingUserWithUsername != null) throw new CustomException(400, "Username", "Username is taken, please try a different Username");

            //Checking for Email
            ApplicationUser existingUserWithEmail = await _userManager.FindByEmailAsync(email);
            if (existingUserWithEmail != null) throw new CustomException(400, "Email", "This email address has already been registered.");
        }
        
        public static async Task<ApplicationUser> FindUserByUsernameOrEmailAsync(UserManager<ApplicationUser> _userManager,string usernameOrEmail)
        {
            // Attempt to find the user by username
            var existingUser = await _userManager.FindByNameAsync(usernameOrEmail);

            // If not found, attempt to find by email
            if (existingUser is null)
                existingUser = await _userManager.FindByEmailAsync(usernameOrEmail);

            if (existingUser is null)
                throw new CustomException(400, "Login Failed", "Incorrect username or password");

            return existingUser;
        }
        
        public static async Task<List<ApplicationUser>>CreateAuthQueryAsync(UserManager<ApplicationUser> _userManager,string searchInput)
        {
            //Base
            var query=_userManager.Users.AsQueryable();

            //Search logic
            if (!string.IsNullOrWhiteSpace(searchInput))
                query = query.Where(u => u.Email.Trim().ToLower() == searchInput.Trim().ToLower());

            return await query.ToListAsync();
        }
    }
}
