namespace StudentInfoSystemApp.Core.Entities
{
    public class BaseEntity
    {
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime? UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}
