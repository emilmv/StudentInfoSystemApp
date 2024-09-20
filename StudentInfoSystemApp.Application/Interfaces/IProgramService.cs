using StudentInfoSystemApp.Application.DTOs.ProgramDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IProgramService
    {
        Task<List<ProgramReturnDTO>> GetAllAsync();
        Task<ProgramReturnDTO> GetByIdAsync(int? id);
    }
}
