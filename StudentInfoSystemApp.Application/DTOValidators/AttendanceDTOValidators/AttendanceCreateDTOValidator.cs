using FluentValidation;
using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;

namespace StudentInfoSystemApp.Application.DTOValidators.AttendanceDTOValidators
{
    public class AttendanceCreateDTOValidator : AbstractValidator<AttendanceCreateDTO>
    {
        public AttendanceCreateDTOValidator()
        {
            RuleFor(a => a.AttendanceDate)
            .NotEmpty().WithMessage("Attendance date is required.");

            RuleFor(a => a.Status)
                .NotEmpty().WithMessage("Status is required.");

            RuleFor(a => a.EnrollmentID)
                .GreaterThan(0).WithMessage("Enrollment ID must be a positive number.");
        }
    }
}
