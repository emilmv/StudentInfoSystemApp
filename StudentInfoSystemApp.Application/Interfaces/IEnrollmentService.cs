using StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IEnrollmentService
    {
        Task<List<EnrollmentReturnDTO>> GetAllAsync();
    }
}
