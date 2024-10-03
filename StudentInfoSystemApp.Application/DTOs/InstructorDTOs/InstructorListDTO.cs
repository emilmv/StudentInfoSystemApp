namespace StudentInfoSystemApp.Application.DTOs.InstructorDTOs
{
    public class InstructorListDTO
    {
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => (CurrentPage * 2) < TotalCount;
        public List<InstructorReturnDTO>? Instructors { get; set; }
    }
}
