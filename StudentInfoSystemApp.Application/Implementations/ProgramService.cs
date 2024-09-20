using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.ProgramDTOs;
using StudentInfoSystemApp.Application.Exceptions;
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
        public async Task<ProgramReturnDTO> GetByIdAsync(int? id)
        {
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");
            var program = await _studentInfoSystemContext
                .Programs
                .Include(p=>p.Students)
                .Include (p=>p.Courses)
                .FirstOrDefaultAsync(d => d.ID == id);
            if (program is null) throw new CustomException(400, "ID", $"Program with ID of:'{id}'not found in the database");
            return _mapper.Map<ProgramReturnDTO>(program);
        }
    }
}
