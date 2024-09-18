namespace StudentInfoSystemApp.Core.Entities
{
    public class Schedule
    {
        public int ID { get; set; }
        public string? Semester { get; set; }
        public string? ClassTime { get; set; }
        public string? Classroom { get; set; }
        //Relations below
        public int CourseID { get; set; }
        public Course? Course { get; set; }
        public int InstructorID { get; set; }
        public Instructor? Instructor { get; set; }
    }
}
