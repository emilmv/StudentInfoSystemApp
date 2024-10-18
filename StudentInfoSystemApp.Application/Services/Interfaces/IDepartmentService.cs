using StudentInfoSystemApp.Application.DTOs.DepartmentDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;

namespace StudentInfoSystemApp.Application.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<PaginationListDTO<DepartmentReturnDTO>> GetAllAsync(int page = 1, string searchInput = "", int pageSize = 3);
        Task<DepartmentReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(DepartmentCreateDTO departmentCreateDTO);
        Task<bool> DeleteAsync(int? id);
        Task<UpdateResponseDTO<DepartmentReturnDTO>> UpdateAsync(int? id, string departmentName);
    }
}
