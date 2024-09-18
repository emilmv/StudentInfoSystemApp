using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<DepartmentReturnDTO>> GetAllAsync();
    }
}
