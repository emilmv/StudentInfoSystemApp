namespace StudentInfoSystemApp.Core.Entities
{
    public class Program : BaseEntity
    {
        public int ID { get; set; }
        public string? ProgramName { get; set; }
        public string? Description { get; set; }
        public int RequiredCredits { get; set; }
        //Relations below
        public List<Student>? Students { get; set; }
        public List<Course>? Courses { get; set; }
    }
}
