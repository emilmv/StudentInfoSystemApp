using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudentInfoSystemApp.Application.Helpers.StudentHelpers;

namespace StudentInfoSystemApp.Application.DTOs.StudentDTOs
{
    public class StudentCreateDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string? Status { get; set; }
        public IFormFile? Photo { get; set; }
        public int ProgramID { get; set; }
    }
    public class StudentCreateDTOValidator : AbstractValidator<StudentCreateDTO>
    {
        public StudentCreateDTOValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot be longer than 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot be longer than 50 characters.");

            RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .LessThan(DateTime.Now).WithMessage("Date of birth must be in the past.")
            .Must(ValidationHelper.BeAtLeast16YearsOld).WithMessage("Student must be at least 16 years old.");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required.")
                .Must(ValidationHelper.BeValidGender)
                .WithMessage("Gender must be 'Male' or 'Female'.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.");

            RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+994 \d{2} \d{3} \d{2} \d{2}$").WithMessage("Phone number must be in the format '+994 xx xxx xx xx'.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(100).WithMessage("Address cannot be longer than 100 characters.");

            RuleFor(x => x.EnrollmentDate)
                .NotEmpty().WithMessage("Enrollment date is required.")
                .LessThan(DateTime.Now).WithMessage("Enrollment date must be in the past.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(ValidationHelper.BeValidStatus)
                .WithMessage("Status must be 'Active' or 'Inactive'.");

            RuleFor(i => i)
                .Custom((i, context) =>
                {
                    if (i.Photo.Length / 1024 > 2048) context.AddFailure("Photo", "Photo size cannot exceed 2 MB");
                });

            RuleFor(i => i.Photo)
                .NotEmpty().WithMessage("Photo is required.")
            .Must(ValidationHelper.BeValidFile).WithMessage("Invalid file type. Only .jpg, .jpeg, .png, or .gif are allowed.")
            .When(i => i.Photo != null);

            RuleFor(x => x.ProgramID)
                .GreaterThan(0).WithMessage("Program ID must be greater than 0.");
        }
    }
}
