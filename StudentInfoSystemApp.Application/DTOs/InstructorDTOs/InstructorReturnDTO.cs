namespace StudentInfoSystemApp.Application.DTOs.InstructorDTOs
{
    public class InstructorReturnDTO
    {
        public int ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? HireDate { get; set; }
        public string? Photo { get; set; }
        //Relations below
        public DepartmentInInstructorReturnDTO? Department { get; set; }
        public List<ScheduleInInstructorReturnDTO>? Schedules { get; set; }
    }
    public class DepartmentInInstructorReturnDTO
    {
        public int ID { get; set; }
        public string? DepartmentName { get; set; }
    }
    public class ScheduleInInstructorReturnDTO
    {
        public int ID { get; set; }
        public string? Semester { get; set; }
        public string? ClassTime { get; set; }
        public string? ClassRoom { get; set; }
        //Relations below
        public string? CourseName { get; set; }
    }
}
