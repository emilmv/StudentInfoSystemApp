using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.ScheduleDTOs;
using StudentInfoSystemApp.Application.DTOs.StudentDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Interfaces;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;
        public StudentService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
        }

        public async Task<List<StudentReturnDTO>> GetAllAsync()
        {
            return _mapper.Map<List<StudentReturnDTO>>(await _studentInfoSystemContext
                .Students
                .Include(s=>s.Enrollments)
                .Include(s=>s.Program)
                .ToListAsync());
        }
        public async Task<StudentReturnDTO> GetByIdAsync(int? id)
        {
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");
            var student = await _studentInfoSystemContext
                .Students
                .Include(s => s.Enrollments)
                .Include(s => s.Program)
                .FirstOrDefaultAsync(d => d.ID == id);
            if (student is null) throw new CustomException(400, "ID", $"Student with ID of:'{id}'not found in the database");
            return _mapper.Map<StudentReturnDTO>(student);
        }
    }
}
