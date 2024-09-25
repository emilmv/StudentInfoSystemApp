using StudentInfoSystemApp.Application.DTOs.DepartmentDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<DepartmentReturnDTO>> GetAllAsync();
        Task<DepartmentReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(DepartmentCreateDTO departmentCreateDTO);
    }
}
