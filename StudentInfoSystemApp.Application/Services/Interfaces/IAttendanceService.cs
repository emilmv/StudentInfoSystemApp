using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;

namespace StudentInfoSystemApp.Application.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<PaginationListDTO<AttendanceReturnDTO>> GetAllAsync(int page = 1, string searchInput = "", int pageSize = 3);
        Task<AttendanceReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(AttendanceCreateDTO attendanceCreateDTO);
        Task<bool> DeleteAsync(int? id);
        Task<UpdateResponseDTO<AttendanceReturnDTO>> UpdateAsync(int? id, AttendanceUpdateDTO attendanceUpdateDTO);
    }
}