namespace StudentInfoSystemApp.Core.Entities
{
    public class Course
    {
        public int ID { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public string Description { get; set; }
        public int Credits { get; set; }
        public int ProgramID { get; set; }
    }
}
