using FluentValidation;

namespace StudentInfoSystemApp.Application.DTOs.ScheduleDTOs
{
    public class ScheduleCreateDTO
    {
        public string? Semester { get; set; }
        public string? ClassTime { get; set; }
        public string? Classroom { get; set; }
        public int CourseID { get; set; }
        public int InstructorID { get; set; }
    }
    public class ScheduleCreateDTOValidator : AbstractValidator<ScheduleCreateDTO>
    {
        public ScheduleCreateDTOValidator()
        {
            RuleFor(x => x.Semester)
                .NotEmpty().WithMessage("Semester is required.")
                .Must(BeValidSemester)
                .WithMessage("Semester must be 'Fall', 'Spring', 'Summer', or 'Winter'.");

            RuleFor(x => x.ClassTime)
             .NotEmpty().WithMessage("Class time is required.")
             .Matches(@"^(?:[01]\d|2[0-3]):[0-5]\d - (?:[01]\d|2[0-3]):[0-5]\d$").WithMessage("Class time must be in the format 'HH:mm - HH:mm'.");

            RuleFor(x => x.Classroom)
                .NotEmpty().WithMessage("Classroom is required.")
                .MaximumLength(50).WithMessage("Classroom cannot be longer than 50 characters.");

            RuleFor(x => x.CourseID)
                .GreaterThan(0).WithMessage("Course ID must be greater than 0.");

            RuleFor(x => x.InstructorID)
                .GreaterThan(0).WithMessage("Instructor ID must be greater than 0.");
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
