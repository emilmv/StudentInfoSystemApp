namespace StudentInfoSystemApp.Core.Entities
{
    public class Attendance
    {
        public int ID { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string? Status { get; set; }
        //Relations below
        public int EnrollmentID { get; set; }
        public Enrollment? Enrollment { get; set; }
    }
}
