namespace StudentInfoSystemApp.Core.Entities
{
    public class Instructor
    {
        public int ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime HireDate { get; set; }
        //Relations below
        public int DepartmentID { get; set; }
        public Department? Department { get; set; }
        public List<Schedule>? Schedules { get; set; }
    }
}
