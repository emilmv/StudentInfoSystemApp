using StudentInfoSystemApp.Application.DTOs.DepartmentDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;

namespace StudentInfoSystemApp.Application.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<PaginationListDTO<DepartmentReturnDTO>> GetAllAsync(int page = 1, string searchInput = "");
        Task<DepartmentReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(DepartmentCreateDTO departmentCreateDTO);
        Task<bool> DeleteAsync(int? id);
    }
}
