using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ScheduleDTOs;

namespace StudentInfoSystemApp.Application.Services.Interfaces
{
    public interface IScheduleService
    {
        Task<PaginationListDTO<ScheduleReturnDTO>> GetAllAsync(int page = 1, string searchInput = "");
        Task<ScheduleReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(ScheduleCreateDTO scheduleCreateDTO);
        Task<bool> DeleteAsync(int? id);

    }
}
