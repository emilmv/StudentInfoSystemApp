using StudentInfoSystemApp.Application.DTOs.InstructorDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ProgramDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;

namespace StudentInfoSystemApp.Application.Services.Interfaces
{
    public interface IProgramService
    {
        Task<PaginationListDTO<ProgramReturnDTO>> GetAllAsync(int page = 1, string searchInput = "");
        Task<ProgramReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(ProgramCreateDTO programCreateDTO);
        Task<bool> DeleteAsync(int? id);
        Task<UpdateResponseDTO<ProgramReturnDTO>> UpdateAsync(int? id, ProgramUpdateDTO programUpdateDTO);

    }
}
