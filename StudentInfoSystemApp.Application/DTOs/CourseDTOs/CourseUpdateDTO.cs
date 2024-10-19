using FluentValidation;

namespace StudentInfoSystemApp.Application.DTOs.CourseDTOs
{
    public class CourseUpdateDTO
    {
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public string Description { get; set; }
        public int? Credits { get; set; }
        public int? ProgramID { get; set; }
    }
    public class CourseUpdateDTOValidator : AbstractValidator<CourseUpdateDTO>
    {
        public CourseUpdateDTOValidator()
        {
            RuleFor(c => c.CourseName)
             .MaximumLength(100).WithMessage("Course name must not exceed 100 characters.")
             .MinimumLength(5).WithMessage("Course name must have at least 5 characters.")
             .When(c => !string.IsNullOrEmpty(c.CourseName));

            RuleFor(c => c.CourseCode)
                .Matches("^[A-Z0-9]+$").WithMessage("Course code must be alphanumeric.")
                .When(c=> !string.IsNullOrEmpty(c.CourseCode));

            RuleFor(c => c.Description)
                .MinimumLength(20).WithMessage("Description must be at least 20 characters long.")
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
                .When(c=>!string.IsNullOrEmpty(c.Description));

            RuleFor(c => c.Credits)
                .InclusiveBetween(3, 12).WithMessage("Credits must be between 3 and 12.")
                .When(c=>c.Credits.HasValue&&c.Credits!=0);
        }
    }
}
