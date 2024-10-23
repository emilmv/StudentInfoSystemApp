using FluentValidation;
using StudentInfoSystemApp.Application.DTOs.ProgramDTOs;

namespace StudentInfoSystemApp.Application.DTOValidators.ProgramDTOValidators
{
    public class ProgramUpdateDTOValidator : AbstractValidator<ProgramUpdateDTO>
    {
        public ProgramUpdateDTOValidator()
        {
            RuleFor(p => p.ProgramName)
                .Length(3, 100).WithMessage("Program name must be between 3 and 100 characters.")
                .When(p => !string.IsNullOrWhiteSpace(p.ProgramName));

            RuleFor(p => p.Description)
                .MaximumLength(500).WithMessage("Description cannot be longer than 500 characters.")
                .When(p => !string.IsNullOrWhiteSpace(p.Description));

            RuleFor(p => p.RequiredCredits)
                .GreaterThan(80).WithMessage("Required credits must be greater than 80.")
                .LessThanOrEqualTo(120).WithMessage("Required credits cannot exceed 120.")
                .When(p => p.RequiredCredits.HasValue);
        }
    }
}
