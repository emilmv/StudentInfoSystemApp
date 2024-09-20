using StudentInfoSystemApp.Application.DTOs.StudentDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IStudentService
    {
        Task<List<StudentReturnDTO>> GetAllAsync();
        Task<StudentReturnDTO> GetByIdAsync(int? id);
    }
}
