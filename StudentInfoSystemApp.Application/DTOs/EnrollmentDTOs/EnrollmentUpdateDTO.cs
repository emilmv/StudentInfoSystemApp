namespace StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs
{
    public class EnrollmentUpdateDTO
    {
        public string EnrollmentDate { get; set; }
        public string Grade { get; set; }
        public string Semester { get; set; }
        public int? StudentID { get; set; }
        public int? CourseID { get; set; }
    }
}
