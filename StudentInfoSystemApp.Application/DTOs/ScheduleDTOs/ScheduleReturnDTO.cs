namespace StudentInfoSystemApp.Application.DTOs.ScheduleDTOs
{
    public class ScheduleReturnDTO
    {
        public int ID { get; set; }
        public string? Semester { get; set; }
        public string? ClassTime { get; set; }
        public string? Classroom { get; set; }
        //Relations below
        public CourseInScheduleReturnDTO? Course { get; set; }
        public InstructorInScheduleReturnDTO? Instructor { get; set; }
    }
    public class CourseInScheduleReturnDTO
    {
        public int ID { get; set; }
        public string? CourseName { get; set; }
        public string? CourseCode { get; set; }
        public string? Description { get; set; }
    }
    public class InstructorInScheduleReturnDTO
    {
        public int ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
