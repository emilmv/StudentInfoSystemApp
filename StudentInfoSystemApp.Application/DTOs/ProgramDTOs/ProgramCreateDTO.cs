using FluentValidation;

namespace StudentInfoSystemApp.Application.DTOs.ProgramDTOs
{
    public class ProgramCreateDTO
    {
        public string ProgramName { get; set; }
        public string Description { get; set; }
        public int RequiredCredits { get; set; }
    }
}
