using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StudentInfoSystemApp.Application.DTOs.InstructorDTOs
{
    public class InstructorUpdateDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string HireDate { get; set; }
        //iformfile
        [FromForm]
        public IFormFile PhotoFile { get; set; }
        public int? DepartmentID { get; set; }
    }
}
