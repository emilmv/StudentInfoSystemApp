using StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IEnrollmentService
    {
        Task<EnrollmentListDTO> GetAllAsync(int page = 1, string searchInput = "");
        Task<EnrollmentReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(EnrollmentCreateDTO enrollmentCreateDTO);
    }
}
