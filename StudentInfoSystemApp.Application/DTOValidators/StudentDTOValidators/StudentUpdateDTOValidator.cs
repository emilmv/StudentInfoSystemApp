using FluentValidation;
using StudentInfoSystemApp.Application.DTOs.StudentDTOs;
using StudentInfoSystemApp.Application.Helpers.StudentHelpers;

namespace StudentInfoSystemApp.Application.DTOValidators.StudentDTOValidators
{
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
                .Matches(@"^\+994 \d{2} \d{3} \d{2} \d{2}$").WithMessage("Phone number must be in the format +994 XX XXX XX XX")
                .When(s => !string.IsNullOrEmpty(s.PhoneNumber));

            RuleFor(x => x.Address)
                .MaximumLength(100).WithMessage("Address cannot be longer than 100 characters.")
                .When(s => !string.IsNullOrEmpty(s.Address));

            RuleFor(x => x.Status)
                .Must(ValidationHelper.BeValidStatus)
                .WithMessage("Status must be 'Active', 'Inactive' or 'Graduated'.")
                .When(s => !string.IsNullOrEmpty(s.Status));

            RuleFor(i => i)
                .Custom((i, context) =>
                {
                    if (i.PhotoFile != null && i.PhotoFile.Length / 1024 > 2048) context.AddFailure("Photo", "Photo size cannot exceed 2 MB");
                });

            RuleFor(i => i.PhotoFile)
                .Must(ValidationHelper.BeValidFile).WithMessage("Invalid file type. Only .jpg, .jpeg, .png, or .gif are allowed.")
                .When(i => i.PhotoFile != null);

            RuleFor(x => x.ProgramID)
                .GreaterThan(0).WithMessage("Program ID must be greater than 0.")
                .When(s => s.ProgramID.HasValue);
        }
    }
}
