using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.StudentDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IStudentService
    {
        Task<PaginationListDTO<StudentReturnDTO>> GetAllAsync(int page = 1, string searchInput = "");
        Task<StudentReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(StudentCreateDTO studentCreateDTO);
        Task<bool> DeleteAsync(int? id);
    }
}
