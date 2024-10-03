using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IAttendanceService
    {
        Task<AttendanceListDTO> GetAllAsync(int page=1,string searchInput="");
        Task<AttendanceReturnDTO> GetByIdAsync(int? id);
        Task<int>CreateAsync(AttendanceCreateDTO attendanceCreateDTO);
    }
}