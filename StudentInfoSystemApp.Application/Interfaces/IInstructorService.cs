using StudentInfoSystemApp.Application.DTOs.InstructorDTOs;

namespace StudentInfoSystemApp.Application.Interfaces
{
    public interface IInstructorService
    {
        Task<List<InstructorReturnDTO>> GetAllAsync();
    }
}
