using FluentValidation;

namespace StudentInfoSystemApp.Application.DTOs.CourseDTOs
{
    public class CourseCreateDTO
    {
        public string? CourseName { get; set; }
        public string? CourseCode { get; set; }
        public string? Description { get; set; }
        public int Credits { get; set; }
        public int ProgramID { get; set; }
    }
    //Validators for CourseCreateDTO
    public class CourseCreateDTOValidator:AbstractValidator<CourseCreateDTO>
    {
        public CourseCreateDTOValidator()
        {
            RuleFor(course => course.CourseName)
             .NotEmpty().WithMessage("Course name is required.")
             .MaximumLength(100).WithMessage("Course name must not exceed 100 characters.");

            RuleFor(course => course.CourseCode)
                .NotEmpty().WithMessage("Course code is required.")
                .Matches("^[A-Z0-9]+$").WithMessage("Course code must be alphanumeric.");

            RuleFor(course => course.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MinimumLength(20).WithMessage("Description must be at least 20 characters long.")
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(course => course.Credits)
                .InclusiveBetween(3, 12).WithMessage("Credits must be between 3 and 12.");

            RuleFor(course => course.ProgramID)
                .GreaterThan(0).WithMessage("Program ID must be a positive number.");
        }
    }
}
