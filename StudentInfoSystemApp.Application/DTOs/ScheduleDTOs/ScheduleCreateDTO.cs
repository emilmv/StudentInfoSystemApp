namespace StudentInfoSystemApp.Application.DTOs.ScheduleDTOs
{
    public class ScheduleCreateDTO
    {
        public string Semester { get; set; }
        public string ClassTime { get; set; }
        public string Classroom { get; set; }
        public int CourseID { get; set; }
        public int InstructorID { get; set; }
    }
}
