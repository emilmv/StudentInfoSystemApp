using FluentValidation;
using StudentInfoSystemApp.Application.DTOs.InstructorDTOs;
using StudentInfoSystemApp.Application.Helpers.StudentHelpers;

namespace StudentInfoSystemApp.Application.DTOValidators.InstructorDTOValidators
{
    public class InstructorUpdateDTOValidator : AbstractValidator<InstructorUpdateDTO>
    {
        public InstructorUpdateDTOValidator()
        {
            RuleFor(i => i.FirstName)
                .MinimumLength(2).WithMessage("First name must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.")
                .When(i => !string.IsNullOrEmpty(i.FirstName));

            RuleFor(i => i.LastName)
                .MinimumLength(2).WithMessage("Last name must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.")
                .When(i => !string.IsNullOrEmpty(i.LastName));

            RuleFor(i => i.Email)
                .EmailAddress().WithMessage("Invalid email format.")
                .When(i => !string.IsNullOrEmpty(i.Email));

            RuleFor(i => i.PhoneNumber)
                .Matches(@"^\+994 \d{2} \d{3} \d{2} \d{2}$").WithMessage("Phone number must be in the format +994 XX XXX XX XX")
                .When(i => !string.IsNullOrEmpty(i.PhoneNumber));

            RuleFor(i => i)
                .Custom((i, context) =>
                {
                    if (i.PhotoFile != null && i.PhotoFile.Length / 1024 > 2048) context.AddFailure("Photo", "Photo size cannot exceed 2 MB");
                });

            RuleFor(i => i.PhotoFile)
            .Must(ValidationHelper.BeValidFile).WithMessage("Invalid file type. Only .jpg, .jpeg, .png, or .gif are allowed.")
            .When(i => i.PhotoFile != null);
        }
    }
}
