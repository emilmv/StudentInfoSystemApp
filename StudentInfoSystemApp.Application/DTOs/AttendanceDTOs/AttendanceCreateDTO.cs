using FluentValidation;

namespace StudentInfoSystemApp.Application.DTOs.AttendanceDTOs
{
    public class AttendanceCreateDTO
    {
        public DateTime AttendanceDate { get; set; }
        public string? Status { get; set; }
        public int EnrollmentID { get; set; }
    }
    //Validator for AttendanceCreateDTO
    public class AttendanceCreateDTOValidator:AbstractValidator<AttendanceCreateDTO>
    {
        public AttendanceCreateDTOValidator()
        {
            RuleFor(attendance => attendance.AttendanceDate)
            .NotEmpty().WithMessage("Attendance date is required.");

            RuleFor(attendance => attendance.Status)
                .NotEmpty().WithMessage("Status is required.");

            RuleFor(attendance => attendance.EnrollmentID)
                .GreaterThan(0).WithMessage("Enrollment ID must be a positive number.");
        }
    }
}
