using FluentValidation;

namespace StudentInfoSystemApp.Application.DTOs.AuthDTOs
{
    public class ResendVerificationEmailDTO
    {
        public string? Email { get; set; }
    }
    public class ResendVerificationEmailDTOValidator : AbstractValidator<ResendVerificationEmailDTO>
    {
        public ResendVerificationEmailDTOValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");
        }
    }
}
