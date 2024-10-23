using FluentValidation;

namespace StudentInfoSystemApp.Application.DTOs.CourseDTOs
{
    public class CourseCreateDTO
    {
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public string Description { get; set; }
        public int Credits { get; set; }
        public int ProgramID { get; set; }
    }
}
