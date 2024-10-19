using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.CourseDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Services.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace StudentInfoSystemApp.Application.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;
        private readonly IPaginationService<Course> _paginationService;
        public CourseService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper, IPaginationService<Course> paginationService)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
            _paginationService = paginationService;
        }

        public async Task<PaginationListDTO<CourseReturnDTO>> GetAllAsync(int page = 1, string searchInput = "", int pageSize = 3)
        {
            //Extracting query to not overload requests
            var query = CreateCourseQuery(searchInput);

            //Pagination
            var paginatedCourseDatas = await _paginationService.ApplyPaginationAsync(query, page, pageSize);

            var totalCount = await query.CountAsync();

            return new PaginationListDTO<CourseReturnDTO>
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
                Objects = _mapper.Map<List<CourseReturnDTO>>(paginatedCourseDatas)
            };
        }
        public async Task<CourseReturnDTO> GetByIdAsync(int? id)
        {
            //Validating ID
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");

            //Getting Course
            var course = await GetCourseByIdAsync(id.Value);

            //Return DTO
            return _mapper.Map<CourseReturnDTO>(course);
        }
        public async Task<CreateResponseDTO<CourseReturnDTO>> CreateAsync(CourseCreateDTO courseCreateDTO)
        {
            //Checking if Course exists in the database
            await EnsureCourseDoesNotExistAsync(courseCreateDTO.CourseName);

            //Ensuring that Course code is unique
            await CheckCourseCodeUniquenessAsync(courseCreateDTO.CourseCode);

            //Finding relevant Program
            await EnsureProgramExistsAsync(courseCreateDTO.ProgramID);

            //Mapping DTO to Object
            Course course = _mapper.Map<Course>(courseCreateDTO);

            //Adding the entity to the database
            await _studentInfoSystemContext.Courses.AddAsync(course);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning the ID of the created entity
            return new CreateResponseDTO<CourseReturnDTO>()
            {
                Response = true,
                CreationDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<CourseReturnDTO>(course)
            };
        }
        public async Task<bool> DeleteAsync(int? id)
        {
            //Checking if requested ID is mentioned
            if (id is null) throw new CustomException(400, "ID", "ID cannot be empty");

            //Checking if a Course with requested ID exists in the database
            var existingCourse = _studentInfoSystemContext.Courses.SingleOrDefault(c => c.ID == id);
            if (existingCourse is null) throw new CustomException(400, "ID", $"A Course with ID of: '{id}' not found in the database");

            //Deleting the requested attendance
            _studentInfoSystemContext.Courses.Remove(existingCourse);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning true if delete successful
            return true;
        }
        public async Task<UpdateResponseDTO<CourseReturnDTO>> UpdateAsync(int? id, CourseUpdateDTO courseUpdateDTO)
        {
            //Checking if ID from body is provided
            if (id is null) throw new CustomException(400, "Attendance ID", "Course ID must not be empty");

            //Finding relevant Course with ID
            var existingCourse = await GetCourseByIdAsync(id.Value);

            //Checking if ProgramID is changed to validate existence
            await UpdateProgramIdAsync(courseUpdateDTO.ProgramID, existingCourse.ProgramID);

            //Checking if Coursename is provided
            await UpdateCourseNameAsync(existingCourse, courseUpdateDTO);

            //Checking if CourseCode is provided
            await UpdateCourseCodeAsync(existingCourse, courseUpdateDTO);

            //Changing description if provided
            if (!string.IsNullOrWhiteSpace(courseUpdateDTO.Description))
                existingCourse.Description = courseUpdateDTO.Description; //Just changing because description validation is not necessary

            //Changing credits
            if (courseUpdateDTO.Credits.HasValue && courseUpdateDTO.Credits != 0)
                existingCourse.Credits = courseUpdateDTO.Credits.Value;

            //Changing ProgramID
            existingCourse.ProgramID = courseUpdateDTO.ProgramID.GetValueOrDefault();

            // Save changes
            _studentInfoSystemContext.Update(existingCourse);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Return response DTO
            return new UpdateResponseDTO<CourseReturnDTO>()
            {
                Response = true,
                UpdateDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<CourseReturnDTO>(existingCourse)
            };
        }



        //Private methods
        public IQueryable<Course> CreateCourseQuery(string searchInput)
        {
            var query = _studentInfoSystemContext.Courses
                .Include(c => c.Program)
                .Include(c => c.Enrollments)
                .Include(c => c.Schedules)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchInput))
            {
                query = query.Where(c =>
                    c.CourseName.ToLower().Contains(searchInput.ToLower()) ||
                    c.CourseCode.ToLower().Contains(searchInput.ToLower()) ||
                    c.Description.ToLower().Contains(searchInput.ToLower())
                );
            }
            return query;
        }
        public async Task<Course> GetCourseByIdAsync(int id)
        {
            var course = await _studentInfoSystemContext
                .Courses
                .Include(c => c.Program)
                .Include(c => c.Enrollments)
                .Include(c => c.Schedules)
                .FirstOrDefaultAsync(c => c.ID == id);

            if (course is null) throw new CustomException(400, "ID", $"Course with ID of:'{id}'not found in the database");

            return course;
        }
        public async Task EnsureCourseDoesNotExistAsync(string courseName)
        {
            var existCourse = await _studentInfoSystemContext.Courses.SingleOrDefaultAsync(c => c.CourseName.ToLower() == courseName.ToLower());
            if (existCourse != null)
                throw new CustomException(400, "CourseName", $"Course with name of: '{courseName}' already exists in the database.");
        }
        public async Task CheckCourseCodeUniquenessAsync(string courseCode)
        {
            var existingCourseCode = await _studentInfoSystemContext.Courses.SingleOrDefaultAsync(c => c.CourseCode.Trim().ToLower() == courseCode.Trim().ToLower());
            if (existingCourseCode != null)
                throw new CustomException(400, "CourseCode", $"A course with code of: '{courseCode}' already exists in the database");
        }
        public async Task EnsureProgramExistsAsync(int programID)
        {
            var program = await _studentInfoSystemContext.Programs.SingleOrDefaultAsync(e => e.ID == programID);
            if (program is null)
                throw new CustomException(400, "ID", $"Program with ID of:' {programID}' not found in the database");
        }
        public async Task UpdateProgramIdAsync(int? programID, int existingProgramID)
        {
            if (existingProgramID != programID)
            {
                programID = (programID == null || programID == 0)
                    ? existingProgramID
                    : programID;

                // checking if the ProgramID has changed and validate it
                if (existingProgramID != programID)
                {
                    var existingProgram = await _studentInfoSystemContext.Programs
                        .SingleOrDefaultAsync(p => p.ID == programID);

                    if (existingProgram is null)
                        throw new CustomException(400, "Program ID", $"A Program with ID of: '{programID}' not found in the database");
                }
            }
        }
        public async Task UpdateCourseNameAsync(Course existingCourse, CourseUpdateDTO courseUpdateDTO)
        {
            if (!string.IsNullOrWhiteSpace(courseUpdateDTO.CourseName))
            {
                //Checking if CourseName is available
                var existingCourseWithSameName = await _studentInfoSystemContext.Courses.SingleOrDefaultAsync(c => c.CourseName.Trim().ToLower().Equals(courseUpdateDTO.CourseName.Trim().ToLower()));
                if (existingCourseWithSameName != null && existingCourseWithSameName.ID != existingCourse.ID)
                    throw new CustomException(400, "Course Name", $"A course with name of: '{courseUpdateDTO.CourseName}' already exists in the database");

                //Changing CourseName
                existingCourse.CourseName = courseUpdateDTO.CourseName;
            }
        }
        public async Task UpdateCourseCodeAsync(Course existingCourse, CourseUpdateDTO courseUpdateDTO)
        {
            if (!string.IsNullOrWhiteSpace(courseUpdateDTO.CourseCode))
            {
                //Checking if CourseCode is available
                var existingCourseCode = await _studentInfoSystemContext.Courses.SingleOrDefaultAsync(c => c.CourseCode.Trim().ToLower().Equals(courseUpdateDTO.CourseCode.Trim().ToLower()));
                if (existingCourseCode != null && existingCourseCode != existingCourse)
                    throw new CustomException(400, "Course Code", $"A course with code of: '{courseUpdateDTO.CourseCode}' already exists in the database");

                //Changing CourseCode
                existingCourse.CourseCode = courseUpdateDTO.CourseCode;
            }
        }
    }
}
