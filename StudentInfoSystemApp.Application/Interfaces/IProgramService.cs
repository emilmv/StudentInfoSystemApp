using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ProgramDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IProgramService
    {
        Task<PaginationListDTO<ProgramReturnDTO>> GetAllAsync(int page = 1, string searchInput = "");
        Task<ProgramReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(ProgramCreateDTO programCreateDTO);
    }
}
