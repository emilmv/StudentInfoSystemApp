using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;
using StudentInfoSystemApp.Application.Interfaces;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;
        public CourseService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
        }

        public async Task<List<CourseReturnDTO>> GetAllAsync()
        {
            return _mapper.Map<List<CourseReturnDTO>>(await _studentInfoSystemContext
                .Courses
                .Include(c => c.Program)
                .Include(c => c.Enrollments)
                .Include(c => c.Schedules)
                .ToListAsync());
        }
    }
}
