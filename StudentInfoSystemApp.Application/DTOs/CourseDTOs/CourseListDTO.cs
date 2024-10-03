namespace StudentInfoSystemApp.Application.DTOs.CourseDTOs
{
    public class CourseListDTO
    {
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => (CurrentPage * 2) < TotalCount;
        public List<CourseReturnDTO>? Courses { get; set; }
    }
}
