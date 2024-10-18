using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudentInfoSystemApp.Application.Helpers.StudentHelpers;

namespace StudentInfoSystemApp.Application.DTOs.StudentDTOs
{
    public class StudentUpdateDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? EnrollmentDate { get; set; }
        public string? Status { get; set; }
        public IFormFile? Photo { get; set; }
        public int? ProgramID { get; set; }
    }
    public class StudentUpdateDTOValidator : AbstractValidator<StudentUpdateDTO>
    {
        public StudentUpdateDTOValidator()
        {
            RuleFor(s => s.FirstName)
                .MaximumLength(50).WithMessage("First name cannot be longer than 50 characters.")
                .When(s => !string.IsNullOrEmpty(s.FirstName));

            RuleFor(s => s.LastName)
                .MaximumLength(50).WithMessage("Last name cannot be longer than 50 characters.")
                .When(s => !string.IsNullOrEmpty(s.LastName));

            RuleFor(x => x.Gender)
                .Must(ValidationHelper.BeValidGender)
                .WithMessage("Gender must be 'Male' or 'Female'.")
                .When(s => !string.IsNullOrEmpty(s.Gender));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email must be a valid email address.")
                .When(s => !string.IsNullOrEmpty(s.Email));

            RuleFor(s => s.PhoneNumber)
                .Matches(@"^\+994 \d{2} \d{3} \d{2} \d{2}$").WithMessage("Phone number must be in the format '+994 xx xxx xx xx'.")
                .When(s => !string.IsNullOrEmpty(s.PhoneNumber));

            RuleFor(x => x.Address)
                .MaximumLength(100).WithMessage("Address cannot be longer than 100 characters.")
                .When(s => !string.IsNullOrEmpty(s.Address));

            RuleFor(x => x.Status)
                .Must(ValidationHelper.BeValidStatus)
                .WithMessage("Status must be 'Active', 'Inactive' or 'Graduated'.")
                .When(s=>!string.IsNullOrEmpty(s.Status));

            RuleFor(i => i)
                .Custom((i, context) =>
                {
                    if (i.Photo != null && i.Photo.Length / 1024 > 2048) context.AddFailure("Photo", "Photo size cannot exceed 2 MB");
                });

            RuleFor(i => i.Photo)
                .Must(ValidationHelper.BeValidFile).WithMessage("Invalid file type. Only .jpg, .jpeg, .png, or .gif are allowed.")
                .When(i => i.Photo != null);

            RuleFor(x => x.ProgramID)
                .GreaterThan(0).WithMessage("Program ID must be greater than 0.")
                .When(s => s.ProgramID.HasValue);
        }
    }
}
