using Microsoft.AspNetCore.Http;

namespace StudentInfoSystemApp.Application.DTOs.StudentDTOs
{
    public class StudentCreateDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; }
        public IFormFile PhotoFile { get; set; }
        public int ProgramID { get; set; }
    }
}
