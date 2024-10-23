using FluentValidation;
using StudentInfoSystemApp.Application.DTOs.ProgramDTOs;

namespace StudentInfoSystemApp.Application.DTOValidators.ProgramDTOValidators
{
    public class ProgramCreateDTOValidator : AbstractValidator<ProgramCreateDTO>
    {
        public ProgramCreateDTOValidator()
        {
            RuleFor(x => x.ProgramName)
                .NotEmpty().WithMessage("Program name is required.")
                .Length(3, 100).WithMessage("Program name must be between 3 and 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot be longer than 500 characters.");

            RuleFor(x => x.RequiredCredits)
                .GreaterThan(0).WithMessage("Required credits must be greater than 0.")
                .LessThanOrEqualTo(120).WithMessage("Required credits cannot exceed 120.");
        }
    }
}
