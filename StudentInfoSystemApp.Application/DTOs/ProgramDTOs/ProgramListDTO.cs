namespace StudentInfoSystemApp.Application.DTOs.ProgramDTOs
{
    public class ProgramListDTO
    {
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => (CurrentPage * 2) < TotalCount;
        public List<ProgramReturnDTO>? Programs { get; set; }
    }
}
