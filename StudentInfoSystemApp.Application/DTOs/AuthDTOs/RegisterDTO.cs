using FluentValidation;

namespace StudentInfoSystemApp.Application.DTOs.AuthDTOs
{
    public class RegisterDTO
    {
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? RepeatPassword { get; set; }
    }

    //Validators for RegisterDTO
    public class RegisterDTOValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterDTOValidator()
        {
            RuleFor(r => r.FirstName)
                .NotEmpty().WithMessage("First Name is required.")
                .MaximumLength(50).WithMessage("First Name cannot exceed 50 characters.");

            RuleFor(r => r.LastName)
                .NotEmpty().WithMessage("Last Name is required.")
                .MaximumLength(50).WithMessage("Last Name cannot exceed 50 characters.");

            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid Email format.");

            RuleFor(r => r.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.");

            RuleFor(r => r.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

            RuleFor(r => r.RepeatPassword)
                .Equal(r => r.Password).WithMessage("Passwords do not match.");

            RuleFor(r => r.Password)
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"\d").WithMessage("Password must contain at least one number.")
                .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");
        }
    }
}
