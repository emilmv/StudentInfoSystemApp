namespace StudentInfoSystemApp.Application.DTOs.AttendanceDTOs
{
    public class AttendanceListDTO
    {
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage =>(CurrentPage*2)<TotalCount;
        public List<AttendanceReturnDTO>? Attendances { get; set; }
    }
}
