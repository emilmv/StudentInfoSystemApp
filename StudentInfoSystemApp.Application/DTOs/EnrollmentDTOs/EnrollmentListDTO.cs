using StudentInfoSystemApp.Application.DTOs.DepartmentDTOs;

namespace StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs
{
    public class EnrollmentListDTO
    {
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => (CurrentPage * 2) < TotalCount;
        public List<EnrollmentReturnDTO>? Enrollments { get; set; }
    }
}
