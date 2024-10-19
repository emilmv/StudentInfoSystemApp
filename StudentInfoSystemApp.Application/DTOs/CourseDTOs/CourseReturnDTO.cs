namespace StudentInfoSystemApp.Application.DTOs.CourseDTOs
{
    public class CourseReturnDTO
    {
        public int ID { get; set; }
        public string? CourseName { get; set; }
        public string? CourseCode { get; set; }
        public string? Description { get; set; }
        public int Credits { get; set; }
        public ProgramInCourseReturnDTO? Program { get; set; }
        public int StudentCount { get; set; }
        public List<ScheduleInCourseReturnDTO>? Schedules { get; set; }
    }
    public class ProgramInCourseReturnDTO
    {
        public int ID { get; set; }
        public string? ProgramName { get; set; }
    }
    public class ScheduleInCourseReturnDTO
    {
        public int ID { get; set; }
        public string? Semester { get; set; }
        public string? ClassTime { get; set; }
        public string? Classroom { get; set; }
    }
}
