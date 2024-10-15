namespace StudentInfoSystemApp.Core.Entities
{
    public class Department : BaseEntity
    {
        public int ID { get; set; }
        public string? DepartmentName { get; set; }
        //Relations below
        public List<Instructor>? Instructors { get; set; }
    }
}
