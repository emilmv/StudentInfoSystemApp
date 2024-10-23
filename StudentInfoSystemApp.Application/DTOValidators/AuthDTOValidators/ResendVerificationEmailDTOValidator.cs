using FluentValidation;
using StudentInfoSystemApp.Application.DTOs.AuthDTOs;

namespace StudentInfoSystemApp.Application.DTOValidators.AuthDTOValidators
{
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
