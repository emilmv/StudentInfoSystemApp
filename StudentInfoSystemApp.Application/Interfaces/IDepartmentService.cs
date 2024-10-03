using StudentInfoSystemApp.Application.DTOs.DepartmentDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IDepartmentService
    {
        Task<DepartmentListDTO> GetAllAsync(int page = 1, string searchInput = "");
        Task<DepartmentReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(DepartmentCreateDTO departmentCreateDTO);
    }
}
