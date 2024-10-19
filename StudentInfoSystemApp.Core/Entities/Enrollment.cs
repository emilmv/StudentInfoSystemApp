namespace StudentInfoSystemApp.Core.Entities
{
    public class Enrollment : BaseEntity
    {
        public int ID { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Grade { get; set; }
        public string Semester { get; set; }
        //Relations below
        public int StudentID { get; set; }
        public Student Student { get; set; }
        public int CourseID { get; set; }
        public Course Course { get; set; }
        public List<Attendance> Attendances { get; set; }
    }
}
