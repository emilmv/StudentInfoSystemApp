using StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IEnrollmentService
    {
        Task<List<EnrollmentReturnDTO>> GetAllAsync();
        Task<EnrollmentReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(EnrollmentCreateDTO enrollmentCreateDTO);
    }
}
