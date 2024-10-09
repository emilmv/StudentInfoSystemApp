namespace StudentInfoSystemApp.Application.DTOs.DepartmentDTOs
{
    public class DepartmentListDTO
    {
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => (CurrentPage * 2) < TotalCount;
        public List<DepartmentReturnDTO>? Departments { get; set; }
    }
}
