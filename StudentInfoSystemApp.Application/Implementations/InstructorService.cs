using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.InstructorDTOs;
using StudentInfoSystemApp.Application.Exceptions;
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

        public async Task<InstructorReturnDTO> GetByIdAsync(int? id)
        {
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");
            var instructor = await _studentInfoSystemContext
                .Instructors
                .Include(i => i.Department)
                .Include(i => i.Schedules)
                .ThenInclude(ii=>ii.Course)
                .FirstOrDefaultAsync(d => d.ID == id);
            if (instructor is null) throw new CustomException(400, "ID", $"Instructor with ID of:'{id}'not found in the database");
            return _mapper.Map<InstructorReturnDTO>(instructor);
        }

        public async Task<int> CreateAsync(InstructorCreateDTO ınstructorCreateDTO)
        {
            throw new NotImplementedException();
        }
    }
}
