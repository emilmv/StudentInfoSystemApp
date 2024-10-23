using FluentValidation;
using StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs;

namespace StudentInfoSystemApp.Application.DTOValidators.EnrollmentDTOValidators
{
    public class EnrollmentUpdateDTOValidator : AbstractValidator<EnrollmentUpdateDTO>
    {
        public EnrollmentUpdateDTOValidator()
        {
            RuleFor(s => s.Grade)
                .Matches(@"^[ABCDF][+-]?$")
                .When(s => !string.IsNullOrEmpty(s.Grade))
                .WithMessage("Grade must be a valid letter grade (A, B, C, D, F, optionally with + or -).");

            RuleFor(x => x.Semester)
             .Must(s => s.Equals("Fall", StringComparison.OrdinalIgnoreCase) ||
                        s.Equals("Spring", StringComparison.OrdinalIgnoreCase) ||
                        s.Equals("Summer", StringComparison.OrdinalIgnoreCase))
             .WithMessage("Semester must be either 'Fall', 'Spring', or 'Summer'.")
             .When(s => !string.IsNullOrEmpty(s.Semester));
        }
    }
}
