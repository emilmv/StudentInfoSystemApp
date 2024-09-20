using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.CourseDTOs;
using StudentInfoSystemApp.Application.Exceptions;
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

        public async Task<CourseReturnDTO> GetByIdAsync(int? id)
        {
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");
            var course = await _studentInfoSystemContext
                .Courses
                .Include(c=>c.Program)
                .Include (c=>c.Enrollments)
                .Include(c=>c.Schedules)
                .FirstOrDefaultAsync(c => c.ID == id);
            if (course is null) throw new CustomException(400, "ID", $"Course with ID of:'{id}'not found in the database");
            return _mapper.Map<CourseReturnDTO>(course);
        }
    }
}
