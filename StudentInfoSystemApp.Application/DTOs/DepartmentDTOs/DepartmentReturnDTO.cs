namespace StudentInfoSystemApp.Application.DTOs.DepartmentDTOs
{
    public class DepartmentReturnDTO
    {
        public int ID { get; set; }
        public string? DepartmentName { get; set; }
        public List<InstructorsInDepartmentReturnDTO>? Instructors { get; set; }
    }
    public class InstructorsInDepartmentReturnDTO
    {
        public int ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? HireDate { get; set; }
    }
}
