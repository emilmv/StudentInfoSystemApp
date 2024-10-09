using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;

namespace StudentInfoSystemApp.Application.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<PaginationListDTO<AttendanceReturnDTO>> GetAllAsync(int page = 1, string searchInput = "");
        Task<AttendanceReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(AttendanceCreateDTO attendanceCreateDTO);
        Task<bool> DeleteAsync(int? id);
    }
}