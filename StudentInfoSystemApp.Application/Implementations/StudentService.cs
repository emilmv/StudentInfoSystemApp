using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.StudentDTOs;
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
    }
}
