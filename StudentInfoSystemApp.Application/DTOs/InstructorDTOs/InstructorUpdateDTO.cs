using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentInfoSystemApp.Application.Helpers.StudentHelpers;

namespace StudentInfoSystemApp.Application.DTOs.InstructorDTOs
{
    public class InstructorUpdateDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string HireDate { get; set; }
        //iformfile
        [FromForm]
        public IFormFile Photo { get; set; }
        public int? DepartmentID { get; set; }
    }
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
                .When(i=>!string.IsNullOrEmpty(i.Email));

            RuleFor(i => i.PhoneNumber)
                .Matches(@"^\+?\d{10,15}$").WithMessage("Invalid phone number format.")
                .When(i => !string.IsNullOrEmpty(i.PhoneNumber));

            RuleFor(i => i)
                .Custom((i, context) =>
                {
                    if (i.Photo != null && i.Photo.Length / 1024 > 2048) context.AddFailure("Photo", "Photo size cannot exceed 2 MB");
                });

            RuleFor(i => i.Photo)
            .Must(ValidationHelper.BeValidFile).WithMessage("Invalid file type. Only .jpg, .jpeg, .png, or .gif are allowed.")
            .When(i => i.Photo != null);
        }
    }
}
