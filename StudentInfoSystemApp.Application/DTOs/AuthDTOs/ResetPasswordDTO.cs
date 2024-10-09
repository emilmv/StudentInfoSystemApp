using FluentValidation;

namespace StudentInfoSystemApp.Application.DTOs.AuthDTOs
{
    public class ResetPasswordDTO
    {
        public string? Password { get; set; }
        public string? RepeatPassword { get; set; }
    }

    public class ResetPasswordDTOValidator : AbstractValidator<ResetPasswordDTO>
    {
        public ResetPasswordDTOValidator()
        {
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.RepeatPassword)
                .NotEmpty().WithMessage("Repeat password is required.")
                .Equal(x => x.Password).WithMessage("Passwords do not match.");
        }
    }
}
