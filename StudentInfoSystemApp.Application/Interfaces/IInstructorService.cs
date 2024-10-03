using StudentInfoSystemApp.Application.DTOs.InstructorDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IInstructorService
    {
        Task<InstructorListDTO> GetAllAsync(int page = 1, string searchInput = "");
        Task<InstructorReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(InstructorCreateDTO instructorCreateDTO);
    }
}
