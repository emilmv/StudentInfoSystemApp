using FluentValidation;
using StudentInfoSystemApp.Application.DTOs.DepartmentDTOs;

namespace StudentInfoSystemApp.Application.DTOValidators.DepartmentDTOValidators
{
    public class DepartmentCreateDTOValidator : AbstractValidator<DepartmentCreateDTO>
    {
        public DepartmentCreateDTOValidator()
        {
            RuleFor(x => x.DepartmentName)
                .NotEmpty().WithMessage("Department name is required.")
                .MaximumLength(100).WithMessage("Department name must not exceed 100 characters.")
                .Matches("^[a-zA-Z0-9 ]*$").WithMessage("Department name can only contain letters, numbers, and spaces.");
        }
    }
}
