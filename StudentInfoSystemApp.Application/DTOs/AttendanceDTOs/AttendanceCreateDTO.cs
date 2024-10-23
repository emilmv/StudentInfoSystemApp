namespace StudentInfoSystemApp.Application.DTOs.AttendanceDTOs
{
    public class AttendanceCreateDTO
    {
        public DateTime AttendanceDate { get; set; }
        public string Status { get; set; }
        public int EnrollmentID { get; set; }
    }
}
