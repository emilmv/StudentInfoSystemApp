namespace StudentInfoSystemApp.Core.Entities
{
    public class Enrollment
    {
        public int ID { get; set; }
        public int StudentID { get; set; }
        public int CourseID { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Grade { get; set; }
        public string Semester { get; set; }
    }
}
