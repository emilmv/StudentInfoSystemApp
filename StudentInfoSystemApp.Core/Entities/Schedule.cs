namespace StudentInfoSystemApp.Core.Entities
{
    public class Schedule
    {
        public int ScheduleID { get; set; }
        public int CourseID { get; set; }
        public int InstructorID { get; set; }
        public string Semester { get; set; }
        public string ClassTime { get; set; }
        public string Classroom { get; set; }
    }
}
