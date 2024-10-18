using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;
using StudentInfoSystemApp.Application.DTOs.StudentDTOs;

namespace StudentInfoSystemApp.Application.Services.Interfaces
{
    public interface IStudentService
    {
        Task<PaginationListDTO<StudentReturnDTO>> GetAllAsync(int page = 1, string searchInput = "", int pageSize = 3);
        Task<StudentReturnDTO> GetByIdAsync(int? id);
        Task<CreateResponseDTO<StudentReturnDTO>> CreateAsync(StudentCreateDTO studentCreateDTO);
        Task<bool> DeleteAsync(int? id);
        Task<UpdateResponseDTO<StudentReturnDTO>> UpdateAsync(int? id, StudentUpdateDTO studentUpdateDTO);
    }
}
