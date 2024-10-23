using FluentValidation;
using Microsoft.AspNetCore.Http;
using StudentInfoSystemApp.Application.DTOs.InstructorDTOs;

namespace StudentInfoSystemApp.Application.DTOValidators.InstructorDTOValidators
{
    public class InstructorCreateDTOValidator : AbstractValidator<InstructorCreateDTO>
    {
        public InstructorCreateDTOValidator()
        {
            RuleFor(i => i.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MinimumLength(2).WithMessage("First name must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

            RuleFor(i => i.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MinimumLength(2).WithMessage("Last name must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

            RuleFor(i => i.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(i => i.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+994 \d{2} \d{3} \d{2} \d{2}$").WithMessage("Phone number must be in the format +994 XX XXX XX XX");

            RuleFor(i => i.HireDate)
                .NotEmpty().WithMessage("Hire date is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Hire date cannot be in the future.");

            RuleFor(i => i.PhotoFile)
               .NotEmpty().WithMessage("Photo is required.")
               .Must(BeValidFile).WithMessage("Invalid file type. Only .jpg, .jpeg, .png, or .gif are allowed.")
               .When(i => i.PhotoFile != null);

            RuleFor(i => i.PhotoFile)
                .NotEmpty().WithMessage("Photo is required.");

            RuleFor(i => i)
                .Custom((i, context) =>
                {
                    if (i.PhotoFile != null && i.PhotoFile.Length / 1024 > 2048)
                        context.AddFailure("Photo", "Photo size cannot exceed 2 MB");
                });

            RuleFor(i => i.DepartmentID)
                .GreaterThan(0).WithMessage("Department ID must be greater than 0.");
        }
        private bool BeValidFile(IFormFile file)
        {
            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();
            return validExtensions.Contains(fileExtension);
        }
    }
}
