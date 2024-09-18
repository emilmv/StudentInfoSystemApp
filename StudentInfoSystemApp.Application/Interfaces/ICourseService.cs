using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface ICourseService
    {
        Task<List<CourseReturnDTO>> GetAllAsync();
    }
}
