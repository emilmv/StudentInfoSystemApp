using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.DTOs.ProgramDTOs
{
    public class ProgramReturnDTO
    {
        public int ID { get; set; }
        public string? ProgramName { get; set; }
        public string? Description { get; set; }
        public int RequiredCredits { get; set; }
        //Relations below
        public List<StudentInProgramReturnDTO>? Students { get; set; }
        public List<CourseInProgramReturnDTO>? Courses { get; set; }
    }
    public class StudentInProgramReturnDTO
    {
        public int ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
    public class CourseInProgramReturnDTO
    {
        public int ID { get; set; }
        public string? CourseName { get; set; }
        public string? CourseCode { get; set; }
        public string? Description { get; set; }
    }
}
