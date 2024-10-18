using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;
using StudentInfoSystemApp.Application.DTOs.ScheduleDTOs;

namespace StudentInfoSystemApp.Application.Services.Interfaces
{
    public interface IScheduleService
    {
        Task<PaginationListDTO<ScheduleReturnDTO>> GetAllAsync(int page = 1, string searchInput = "",int pageSize=3);
        Task<ScheduleReturnDTO> GetByIdAsync(int? id);
        Task<CreateResponseDTO<ScheduleReturnDTO>> CreateAsync(ScheduleCreateDTO scheduleCreateDTO);
        Task<bool> DeleteAsync(int? id);
        Task<UpdateResponseDTO<ScheduleReturnDTO>> UpdateAsync(int? id, ScheduleUpdateDTO scheduleUpdateDTO);
    }
}
