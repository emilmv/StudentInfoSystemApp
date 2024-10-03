namespace StudentInfoSystemApp.Application.DTOs.StudentDTOs
{
    public class StudentListDTO
    {
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => (CurrentPage * 2) < TotalCount;
        public List<StudentReturnDTO>? Students { get; set; }
    }
}
