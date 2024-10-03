using StudentInfoSystemApp.Application.DTOs.ProgramDTOs;
using StudentInfoSystemApp.Application.DTOs.ScheduleDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IScheduleService
    {
        Task<ScheduleListDTO> GetAllAsync(int page = 1, string searchInput = "");
        Task<ScheduleReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(ScheduleCreateDTO scheduleCreateDTO);
    }
}
