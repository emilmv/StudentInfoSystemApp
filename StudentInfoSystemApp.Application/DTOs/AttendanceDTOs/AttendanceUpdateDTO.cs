namespace StudentInfoSystemApp.Application.DTOs.AttendanceDTOs
{
    public class AttendanceUpdateDTO
    {
        public string AttendanceDate { get; set; }
        public string Status { get; set; }
        public int? EnrollmentID { get; set; }
    }
}
