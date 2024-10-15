using StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs;
using StudentInfoSystemApp.Application.DTOs.InstructorDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;

namespace StudentInfoSystemApp.Application.Services.Interfaces
{
    public interface IInstructorService
    {
        Task<PaginationListDTO<InstructorReturnDTO>> GetAllAsync(int page = 1, string searchInput = "");
        Task<InstructorReturnDTO> GetByIdAsync(int? id);
        Task<int> CreateAsync(InstructorCreateDTO instructorCreateDTO);
        Task<bool> DeleteAsync(int? id);
        Task<UpdateResponseDTO<InstructorReturnDTO>> UpdateAsync(int? id, InstructorUpdateDTO instructorUpdateDTO);
    }
}
