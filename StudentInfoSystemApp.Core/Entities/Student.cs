namespace StudentInfoSystemApp.Core.Entities
{
    public class Student
    {
        public int ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string? Status { get; set; }
        public string? Photo {  get; set; }
        //Relations below
        public int ProgramID { get; set; }
        public Program? Program { get; set; }
        public List<Enrollment>? Enrollments { get; set; }
    }
}
