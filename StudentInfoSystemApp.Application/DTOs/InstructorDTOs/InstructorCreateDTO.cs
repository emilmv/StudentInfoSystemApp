using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.DTOs.InstructorDTOs
{
    public class InstructorCreateDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime HireDate { get; set; }
       //iformfile
        public string? Photo { get; set; }
        //Relations below
        public int DepartmentID { get; set; }
    }
}
