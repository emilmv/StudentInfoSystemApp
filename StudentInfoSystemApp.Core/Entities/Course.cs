namespace StudentInfoSystemApp.Core.Entities
{
    public class Course : BaseEntity
    {
        public int ID { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public string Description { get; set; }
        public int Credits { get; set; }
        //Relations below
        public int ProgramID { get; set; }
        public Program Program { get; set; }
        public List<Enrollment> Enrollments { get; set; }
        public List<Schedule> Schedules { get; set; }
    }
}
