namespace StudentInfoSystemApp.Application.DTOs.StudentDTOs
{
    public class StudentReturnDTO
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string EnrollmentDate { get; set; }
        public string Status { get; set; }
        public string Photo { get; set; }
        //Relations below
        public ProgramInStudentReturnDTO Program { get; set; }
        public List<EnrollmentInStudentReturnDTO> Enrollments { get; set; }
    }
    public class ProgramInStudentReturnDTO
    {
        public int ID { get; set; }
        public string ProgramName { get; set; }
        public string Description { get; set; }
    }       
    public class EnrollmentInStudentReturnDTO
    {
        public int ID { get; set; }
        public string CourseName { get; set; }
        public string CourseRegistrationDate { get; set; }
        public string Grade { get; set; }
        public string Semester { get; set; }
    }
}
