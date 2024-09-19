using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.InstructorDTOs;
using StudentInfoSystemApp.Application.Interfaces;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Implementations
{
    public class InstructorService : IInstructorService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;

        public InstructorService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
        }


        public async Task<List<InstructorReturnDTO>> GetAllAsync()
        {
            return _mapper.Map<List<InstructorReturnDTO>>(await _studentInfoSystemContext
                .Instructors
                .Include(i => i.Department)
                .Include(i => i.Schedules)
                .ThenInclude(ii=>ii.Course)
                .ToListAsync());
        }
    }
}
