using StudentInfoSystemApp.Application.DTOs.DepartmentDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<DepartmentReturnDTO>> GetAllAsync();
    }
}
