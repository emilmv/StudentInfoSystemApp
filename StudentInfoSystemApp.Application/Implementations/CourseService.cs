using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.CourseDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Interfaces;
using StudentInfoSystemApp.Core.Entities;
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
        public async Task<int> CreateAsync(CourseCreateDTO courseCreateDTO)
        {
            //Extracting query into a variable not to use 2 requests in 1 method
            var query = _studentInfoSystemContext.Courses;

            //Checking if Course exists in the database
            var existCourse= await query.SingleOrDefaultAsync(c=>c.CourseName.ToLower() == courseCreateDTO.CourseName.ToLower());
            if (existCourse != null)
                throw new CustomException(400, "CourseName", $"Course with name of: '{courseCreateDTO.CourseName}' already exists in the database.");

            //Ensuring that Course code is unique
            var existingCourseCode = await query.SingleOrDefaultAsync(c => c.CourseCode.ToLower() == courseCreateDTO.CourseCode.ToLower());
            if (existingCourseCode != null)
                throw new CustomException(400, "CourseCode", $"A course with code of: '{courseCreateDTO.CourseCode}' already exists in the database");
            
            //Finding relevant Program
            var program = await _studentInfoSystemContext.Programs.SingleOrDefaultAsync(e => e.ID == courseCreateDTO.ProgramID);
            if (program is null)
                throw new CustomException(400, "ID", $"Program with ID of:' {courseCreateDTO.ProgramID}' not found in the database");

            //Mapping DTO to Object
            Course course = _mapper.Map<Course>(courseCreateDTO);

            //Adding the entity to the database
            await _studentInfoSystemContext.Courses.AddAsync(course);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning the ID of the created entity
            return course.ID;
        }
    }
}
