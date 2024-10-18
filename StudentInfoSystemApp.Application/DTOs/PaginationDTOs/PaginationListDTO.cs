namespace StudentInfoSystemApp.Application.DTOs.PaginationDTOs
{
    public class PaginationListDTO<T>
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => (CurrentPage * 2) < TotalCount;
        public List<T>? Objects { get; set; }
    }
}
