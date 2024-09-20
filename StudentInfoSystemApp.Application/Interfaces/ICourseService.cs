using StudentInfoSystemApp.Application.DTOs.CourseDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface ICourseService
    {
        Task<List<CourseReturnDTO>> GetAllAsync();
        Task<CourseReturnDTO> GetByIdAsync(int? id);
    }
}
