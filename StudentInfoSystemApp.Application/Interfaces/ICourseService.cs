using StudentInfoSystemApp.Application.DTOs.CourseDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface ICourseService
    {
        Task<CourseListDTO> GetAllAsync(int page = 1, string searchInput = "");
        Task<CourseReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(CourseCreateDTO courseCreateDTO);
    }
}
