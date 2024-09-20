using StudentInfoSystemApp.Application.DTOs.ScheduleDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IScheduleService
    {
        Task<List<ScheduleReturnDTO>> GetAllAsync();
        Task<ScheduleReturnDTO> GetByIdAsync(int? id);
    }
}
