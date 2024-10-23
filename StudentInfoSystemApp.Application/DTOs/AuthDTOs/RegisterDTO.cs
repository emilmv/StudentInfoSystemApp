using FluentValidation;

namespace StudentInfoSystemApp.Application.DTOs.AuthDTOs
{
    public class RegisterDTO
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
    }

    //Validators for RegisterDTO
    
}
