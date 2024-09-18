using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IEnrollmentService
    {
        Task<List<EnrollmentReturnDTO>> GetAllAsync();
    }
}
