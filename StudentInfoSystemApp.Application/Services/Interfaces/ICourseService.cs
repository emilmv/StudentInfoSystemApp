using StudentInfoSystemApp.Application.DTOs.CourseDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;

namespace StudentInfoSystemApp.Application.Services.Interfaces
{
    public interface ICourseService
    {
        Task<PaginationListDTO<CourseReturnDTO>> GetAllAsync(int page = 1, string searchInput = "", int pageSize = 3);
        Task<CourseReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(CourseCreateDTO courseCreateDTO);
        Task<bool> DeleteAsync(int? id);
        Task<UpdateResponseDTO<CourseReturnDTO>> UpdateAsync(int? id, CourseUpdateDTO courseUpdateDTO);
    }
}
