using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudentInfoSystemApp.Application.DTOs.AuthDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IConfiguration configuration, IJwtService jwtService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _jwtService = jwtService;
        }

        public async Task<bool> RegisterAsync(RegisterDTO registerDTO)
        {
            //Checking if username is available by Username
            ApplicationUser existingUser = await _userManager.FindByNameAsync(registerDTO.Username);
            if (existingUser != null) throw new CustomException(400, "Username", "Username is taken, please try a different Username");

            //Checking if username is available by Email
            existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (existingUser != null) throw new CustomException(400, "Email", "This email address has already been registered.");

            //Mapping dto to an object
            ApplicationUser newUser = _mapper.Map<ApplicationUser>(registerDTO);

            //Hashing password
            var hasher = new PasswordHasher<ApplicationUser>();
            newUser.PasswordHash = hasher.HashPassword(newUser, registerDTO.Password);

            //Extracting the result into a variable to handle errors
            IdentityResult result = await _userManager.CreateAsync(newUser);

            //Checking if Registration is failed
            if (!result.Succeeded)
            {
                var exception = new CustomException(400, "Register failed");

                foreach (var error in result.Errors)
                {
                    exception.Errors.Add(error.Code, error.Description);
                }
                throw exception;
            }

            //Assigning User role to registered person
            await _userManager.AddToRoleAsync(newUser, "User");

            return true;
        }

        public async Task<string> LoginAsync(LoginDTO loginDTO)
        {
            //Checking by username
            ApplicationUser existingUser = await _userManager.FindByNameAsync(loginDTO.UsernameOrEmail);
            if (existingUser is null)
                //Checking by email
                existingUser = await _userManager.FindByEmailAsync(loginDTO.UsernameOrEmail);

            if (existingUser is null) throw new CustomException(400, "Login Failed", "Incorrect username or password");

            //Checking if password is correct
            bool isAuthenticated = await _userManager.CheckPasswordAsync(existingUser, loginDTO.Password);
            if (!isAuthenticated)
                throw new CustomException(400, "Login Failed", "Incorrect username or password");

            //Token generation
            var userRoles = await _userManager.GetRolesAsync(existingUser);

            return _jwtService.GenerateToken(existingUser, userRoles.ToList());
        }

        public async Task<List<UserReturnDTO>> GetUsersAsync()
        {
            //Getting all users
            var users = await _userManager.Users.ToListAsync();
            List<UserReturnDTO> usersWithRoles = new();

            //Getting roles of users
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userDTO = _mapper.Map<UserReturnDTO>(user);
                userDTO.Roles = roles;
                usersWithRoles.Add(userDTO);
            }
            return usersWithRoles;
        }

        public async Task<bool> DeleteAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new CustomException(400, "UserNotFound", $"User with ID of: '{userId}' not found in the database.");
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => $"{e.Code}: {e.Description}");
                throw new CustomException(500, "DeleteFailed", string.Join("; ", errors));
            }

            return true;
        }
    }
}
