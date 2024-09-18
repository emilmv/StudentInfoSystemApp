using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.DTOs.AttendanceDTOs
{
    public class InstructorReturnDTO
    {
        public int ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? HireDate { get; set; }
        //Relations below
        public int DepartmentID { get; set; }
        public Department? Department { get; set; }
        public List<Schedule>? Schedules { get; set; }
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
        public string? Classroom { get; set; }
        //Relations below
        public Course? Course { get; set; }
    }
}
