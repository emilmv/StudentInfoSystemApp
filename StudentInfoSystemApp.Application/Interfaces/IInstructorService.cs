using StudentInfoSystemApp.Application.DTOs.InstructorDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IInstructorService
    {
        Task<PaginationListDTO<InstructorReturnDTO>> GetAllAsync(int page = 1, string searchInput = "");
        Task<InstructorReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(InstructorCreateDTO instructorCreateDTO);
        Task<bool> DeleteAsync(int? id);

    }
}
