namespace StudentInfoSystemApp.Core.Entities
{
    public class Instructor
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime HireDate { get; set; }
        public int DepartmentID { get; set; }
    }
}
