using FluentValidation;
using StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs;

namespace StudentInfoSystemApp.Application.DTOValidators.EnrollmentDTOValidators
{
    public class EnrollmentCreateDTOValidator : AbstractValidator<EnrollmentCreateDTO>
    {
        public EnrollmentCreateDTOValidator()
        {
            RuleFor(x => x.EnrollmentDate)
                .NotEmpty()
                .WithMessage("Enrollment date is required.");

            RuleFor(x => x.Grade)
                .Matches(@"^[ABCDF][+-]?$")
                .When(x => !string.IsNullOrEmpty(x.Grade))
                .WithMessage("Grade must be a valid letter grade (A, B, C, D, F, optionally with + or -).");

            RuleFor(x => x.Semester)
             .NotEmpty().WithMessage("Semester is required.")
             .Must(s => s.Equals("Fall", StringComparison.OrdinalIgnoreCase) ||
                        s.Equals("Spring", StringComparison.OrdinalIgnoreCase) ||
                        s.Equals("Summer", StringComparison.OrdinalIgnoreCase))
             .WithMessage("Semester must be either 'Fall', 'Spring', or 'Summer'.");

            RuleFor(x => x.StudentID)
                .GreaterThan(0)
                .WithMessage("StudentID must be a positive number.");

            RuleFor(x => x.CourseID)
                .GreaterThan(0)
                .WithMessage("CourseID must be a positive number.");
        }
    }
}
