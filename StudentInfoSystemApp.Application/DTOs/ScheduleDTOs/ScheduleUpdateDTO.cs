using FluentValidation;

namespace StudentInfoSystemApp.Application.DTOs.ScheduleDTOs
{
    public class ScheduleUpdateDTO
    {
        public string? Semester { get; set; }
        public string? ClassTime { get; set; }
        public string? Classroom { get; set; }
        public int? CourseID { get; set; }
        public int? InstructorID { get; set; }
    }
    public class ScheduleUpdateDTOValidator : AbstractValidator<ScheduleUpdateDTO>
    {
        public ScheduleUpdateDTOValidator()
        {
            RuleFor(s => s.Semester)
               .Must(BeValidSemester)
               .WithMessage("Semester must be 'Fall', 'Spring', 'Summer', or 'Winter'.")
               .When(s => !string.IsNullOrWhiteSpace(s.Semester));

            RuleFor(s => s.ClassTime)
             .Matches(@"^(?:[01]\d|2[0-3]):[0-5]\d - (?:[01]\d|2[0-3]):[0-5]\d$").WithMessage("Class time must be in the format 'HH:mm - HH:mm'.")
             .When(s => !string.IsNullOrWhiteSpace(s.ClassTime));

            RuleFor(s => s.Classroom)
                .MaximumLength(50).WithMessage("Classroom cannot be longer than 50 characters.")
                .When(s => !string.IsNullOrEmpty(s.Classroom));

            RuleFor(s => s.CourseID)
                .GreaterThan(0).WithMessage("Course ID must be greater than 0.")
                .When(s => s.CourseID.HasValue);

            RuleFor(s => s.InstructorID)
                .GreaterThan(0).WithMessage("Instructor ID must be greater than 0.")
                .When(s => s.InstructorID.HasValue);
        }
        private bool BeValidSemester(string? semester)
        {
            return string.Equals(semester, "Fall", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(semester, "Spring", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(semester, "Summer", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(semester, "Winter", StringComparison.OrdinalIgnoreCase);
        }
    }
}
