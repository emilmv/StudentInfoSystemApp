namespace StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs
{
    public class EnrollmentCreateDTO
    {
        public DateTime EnrollmentDate { get; set; }
        public string Grade { get; set; }
        public string Semester { get; set; }
        //Relations below
        public int StudentID { get; set; }
        public int CourseID { get; set; }
    }
}
