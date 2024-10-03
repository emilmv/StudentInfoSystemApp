using StudentInfoSystemApp.Application.DTOs.ScheduleDTOs;
using StudentInfoSystemApp.Application.DTOs.StudentDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IStudentService
    {
        Task<StudentListDTO> GetAllAsync(int page = 1, string searchInput = "");
        Task<StudentReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(StudentCreateDTO studentCreateDTO);
    }
}
