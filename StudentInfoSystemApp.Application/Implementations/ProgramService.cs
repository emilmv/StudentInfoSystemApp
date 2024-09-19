using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.InstructorDTOs;
using StudentInfoSystemApp.Application.DTOs.ProgramDTOs;
using StudentInfoSystemApp.Application.Interfaces;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Implementations
{
    public class ProgramService : IProgramService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;
        public ProgramService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
        }

        public async Task<List<ProgramReturnDTO>> GetAllAsync()
        {
            return _mapper.Map<List<ProgramReturnDTO>>(await _studentInfoSystemContext
                .Programs
                .Include(p=>p.Students)
                .Include(p=>p.Courses)
                .ToListAsync()
                );
        }
    }
}
