namespace StudentInfoSystemApp.Application.DTOs.AttendanceDTOs
{
    public class AttendanceReturnDTO
    {
        public int ID { get; set; }
        public string AttendanceDate { get; set; }
        public EnrollmentInAttendanceReturnDTO Enrollment { get; set; }
    }
    public class EnrollmentInAttendanceReturnDTO
    {
        public int ID { get; set; }
        public string EnrollmentDate { get; set; }
        //Relations below
        public int StudentID { get; set; }
        public string StudentFullName { get; set; }
        public string CourseName { get; set; }
    }
}
