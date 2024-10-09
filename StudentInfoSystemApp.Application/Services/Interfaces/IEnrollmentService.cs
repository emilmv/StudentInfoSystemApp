using StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;

namespace StudentInfoSystemApp.Application.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<PaginationListDTO<EnrollmentReturnDTO>> GetAllAsync(int page = 1, string searchInput = "");
        Task<EnrollmentReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(EnrollmentCreateDTO enrollmentCreateDTO);
        Task<bool> DeleteAsync(int? id);

    }
}
