using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudentInfoSystemApp.Application.DTOs.StudentDTOs;
using StudentInfoSystemApp.Application.Helpers.StudentHelpers;

namespace StudentInfoSystemApp.Application.DTOValidators.StudentDTOValidators
{
    public class StudentCreateDTOValidator : AbstractValidator<StudentCreateDTO>
    {
        public StudentCreateDTOValidator()
        {
            RuleFor(s => s.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot be longer than 50 characters.");

            RuleFor(s => s.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot be longer than 50 characters.");

            RuleFor(s => s.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .LessThan(DateTime.Now).WithMessage("Date of birth must be in the past.")
            .Must(ValidationHelper.BeAtLeast16YearsOld).WithMessage("Student must be at least 16 years old.");

            RuleFor(s => s.Gender)
                .NotEmpty().WithMessage("Gender is required.")
                .Must(ValidationHelper.BeValidGender)
                .WithMessage("Gender must be 'Male' or 'Female'.");

            RuleFor(s => s.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.");

            RuleFor(s => s.PhoneNumber)
            .Matches(@"^\+994 \d{2} \d{3} \d{2} \d{2}$").WithMessage("Phone number must be in the format +994 XX XXX XX XX");

            RuleFor(s => s.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(100).WithMessage("Address cannot be longer than 100 characters.");

            RuleFor(s => s.EnrollmentDate)
                .NotEmpty().WithMessage("Enrollment date is required.")
                .LessThan(DateTime.Now).WithMessage("Enrollment date must be in the past.");

            RuleFor(s => s.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(ValidationHelper.BeValidStatus)
                .WithMessage("Status must be 'Active' or 'Inactive'.");

            RuleFor(s => s.ProgramID)
                .GreaterThan(0).WithMessage("Program ID must be greater than 0.");

            RuleFor(i => i.PhotoFile)
                .NotEmpty().WithMessage("Photo is required.")
                .Must(BeValidFile).WithMessage("Invalid file type. Only .jpg, .jpeg, .png, or .gif are allowed.")
                .When(i => i.PhotoFile != null);

            RuleFor(i => i.PhotoFile)
                .NotEmpty().WithMessage("Photo is required");

            RuleFor(i => i)
                .Custom((i, context) =>
                {
                    if (i.PhotoFile != null && i.PhotoFile.Length / 1024 > 2048)
                        context.AddFailure("Photo", "Photo size cannot exceed 2 MB");
                });
        }
        private bool BeValidFile(IFormFile file)
        {
            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();
            return validExtensions.Contains(fileExtension);
        }
    }
}
