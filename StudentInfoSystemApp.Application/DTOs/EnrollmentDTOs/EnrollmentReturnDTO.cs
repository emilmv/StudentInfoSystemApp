namespace StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs
{
    public class EnrollmentReturnDTO
    {
        public int ID { get; set; }
        public string EnrollmentDate { get; set; }
        public string Grade { get; set; }
        public string Semester { get; set; }
        public StudentInEnrollmentReturnDTO Student { get; set; }
        public CourseInEnrollmentReturnDTO Course { get; set; }
    }
    public class StudentInEnrollmentReturnDTO
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string Photo { get; set; }
    }
    public class CourseInEnrollmentReturnDTO
    {
        public int ID { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
    }
}
