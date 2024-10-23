using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using StudentInfoSystemApp.Application.DTOs.AuthDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Helpers.EntityHelpers;
using StudentInfoSystemApp.Application.Services.Interfaces;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IConfiguration configuration, IJwtService jwtService, IEmailService emailService, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _jwtService = jwtService;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> RegisterAsync(RegisterDTO registerDTO)
        {
            //Checking if username is available by Username and Email
            await AuthHelper.CheckUserAvailabilityAsync(_userManager,registerDTO.Username, registerDTO.Email);

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

            //Generate confirmation link
            var confirmationLink = await _emailService.GenerateEmailConfirmationLinkAsync(newUser);

            //Generate email body
            var body = await _emailService.GenerateVerificationEmailBodyAsync(confirmationLink, newUser.UserName);

            //Send email
            _emailService.SendEmail(new List<string>() { newUser.Email }, "Email Verification", body);

            return "Registration successful. Please check your email to confirm.";
        }
        public async Task<string> LoginAsync(LoginDTO loginDTO)
        {
            //Finding user by username or email
            var existingUser=await AuthHelper.FindUserByUsernameOrEmailAsync(_userManager,loginDTO.UsernameOrEmail);
            
            //Checking if password is correct
            bool isAuthenticated = await _userManager.CheckPasswordAsync(existingUser, loginDTO.Password);
            if (!isAuthenticated)
                throw new CustomException(400, "Login failed", "Incorrect username or password");

            //If email is not confirmed
            if (!existingUser.EmailConfirmed)
                throw new CustomException(400, "Email not confirmed", "Please confirm your email before logging in.");

            //Token generation
            var userRoles = await _userManager.GetRolesAsync(existingUser);

            return _jwtService.GenerateToken(existingUser, userRoles.ToList());
        }
        public async Task<PaginationListDTO<UserReturnDTO>> GetAllAsync(int page = 1, string searchInput = "", int pageSize = 3)
        {
            var query = await AuthHelper.CreateAuthQueryAsync(_userManager, searchInput);

            //Applying pagination manually because paginationService works with IQueryable
            var datas = query.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalCount = query.Count();

            var objects = new List<UserReturnDTO>();

            //Getting roles of users
            foreach (var user in query)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userDTO = _mapper.Map<UserReturnDTO>(user);
                userDTO.Roles = roles;
                objects.Add(userDTO);
            }

            //Returning PaginationDTO
            return new PaginationListDTO<UserReturnDTO>
            {
                TotalCount = totalCount,
                CurrentPage = page,
                Objects = objects
            };
        }
        public async Task<bool> DeleteAsync(string email)
        {
            //Deleting through email because userId is GUID
            if (string.IsNullOrWhiteSpace(email))
                throw new CustomException(400, "Email", "Email is required.");

            // Get the current user's ID
            var currentUserId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            if (currentUserId is null)
                throw new CustomException(400, "User not found", "Please log in to be able to use this operation.");

            // Check if the current user has the "Owner" role
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var roles = await _userManager.GetRolesAsync(currentUser);
            if (!roles.Contains("Owner"))
                throw new CustomException(403, "Permission denied", "You do not have permission to delete users.");

            // Find the user by email
            var userToDelete = await _userManager.FindByEmailAsync(email);
            if (userToDelete is null)
                throw new CustomException(400, "User not found", $"User with email '{email}' not found in the database.");

            // In case the owner decides to delete his own account
            if (currentUserId == userToDelete.Id)
                throw new CustomException(403, "Permission denied", "You cannot delete your own account.");

            // Delete the user
            var result = await _userManager.DeleteAsync(userToDelete);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => $"{e.Code}: {e.Description}");
                throw new CustomException(500, "DeleteFailed", string.Join("; ", errors));
            }

            return true;
        }
        public async Task<string> VerifyEmailAsync(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new CustomException(404, "UserNotFound", "The user does not exist.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => new KeyValuePair<string, string>(e.Code, e.Description));
                throw new CustomException(400, "EmailConfirmationFailed", "Failed to confirm email. Token is invalid or expired");
            }
            //Expire the token
            await _userManager.UpdateSecurityStampAsync(user);

            return "Email verification successful, you can close this window.";
        }
        public async Task<string> ResendVerificationEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new CustomException(404, "UserNotFound", "User not found with the provided email.");

            if (user.EmailConfirmed)
                throw new CustomException(400, "EmailAlreadyConfirmed", "Email is already confirmed. No need to resend the verification link.");

            //Generate confirmation link
            var confirmationLink = await _emailService.GenerateEmailConfirmationLinkAsync(user);

            //Generate email body
            var body = await _emailService.GenerateVerificationEmailBodyAsync(confirmationLink, user.UserName);

            // Send email
            _emailService.SendEmail(new List<string> { user.Email }, "Email Verification", body);

            return "Email sent. Please check your email to confirm.";
        }
        public async Task<string> SendPasswordResetEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                throw new CustomException(400, "UserNotFound", "User with this email does not exist.");

            //Generate reset link
            var resetLink = await _emailService.GenerateResetPasswordLinkAsync(user);

            //Generate email body
            var body = _emailService.GenerateResetPasswordEmailBody(resetLink, user.UserName);

            _emailService.SendEmail(new List<string> { user.Email }, "Password Reset", body);

            return "Email sent, please check your email to confirm and reset your password";
        }
        public async Task<string> ResetPasswordAsync(string email, string token, ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                throw new CustomException(400, "UserNotFound", "User with this email does not exist.");

            var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordDTO.Password);
            if (!result.Succeeded)
                throw new CustomException(400, "ResetPasswordFailed", "Token is invalid or expired");

            //updating security stamp to expire the token
            await _userManager.UpdateSecurityStampAsync(user);

            return "Password reset successful.";
        }
    }
}
