using FluentValidation;

namespace StudentInfoSystemApp.Application.DTOs.AuthDTOs
{
    public class ResetPasswordDTO
    {
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
    }
}
