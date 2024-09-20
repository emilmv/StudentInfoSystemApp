using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IAttendanceService
    {
        Task<List<AttendanceReturnDTO>> GetAllAsync();
        Task<AttendanceReturnDTO> GetByIdAsync(int? id);
    }
}