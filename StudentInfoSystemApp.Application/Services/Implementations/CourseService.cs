using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.CourseDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Helpers.EntityHelpers;
using StudentInfoSystemApp.Application.Services.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;

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
            var query = await CourseHelper.CreateCourseQueryAsync(_studentInfoSystemContext, searchInput);

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
            var responseCourse = await CourseHelper.GetResponseCourseAsync(_studentInfoSystemContext, id.Value);

            //Return DTO
            return _mapper.Map<CourseReturnDTO>(responseCourse);
        }
        public async Task<CreateResponseDTO<CourseReturnDTO>> CreateAsync(CourseCreateDTO courseCreateDTO)
        {
            //Checking if Course exists in the database
            await CourseHelper.EnsureCourseDoesNotExistAsync(_studentInfoSystemContext, courseCreateDTO.CourseName);

            //Ensuring that Course code is unique
            await CourseHelper.CheckCourseCodeUniquenessAsync(_studentInfoSystemContext, courseCreateDTO.CourseCode);

            //Finding relevant Program
            await CourseHelper.EnsureProgramExistsAsync(_studentInfoSystemContext, courseCreateDTO.ProgramID);

            //Mapping DTO to Object
            Course course = _mapper.Map<Course>(courseCreateDTO);

            //Adding the entity to the database
            await _studentInfoSystemContext.Courses.AddAsync(course);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Getting Course
            var responseCourse = await CourseHelper.GetResponseCourseAsync(_studentInfoSystemContext, course.ID);

            //Returning the ID of the created entity
            return new CreateResponseDTO<CourseReturnDTO>()
            {
                Response = true,
                CreationDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<CourseReturnDTO>(responseCourse)
            };
        }
        public async Task<bool> DeleteAsync(int? id)
        {
            //Checking if requested ID is mentioned
            if (id is null) throw new CustomException(400, "ID", "ID cannot be empty");

            //Checking if a Course with requested ID exists in the database
            var existingCourse = await CourseHelper.FindCourseByID(_studentInfoSystemContext, id.Value);

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
            var existingCourse = await CourseHelper.GetResponseCourseAsync(_studentInfoSystemContext, id.Value);

            //Checking if ProgramID is changed to validate existence
            await CourseHelper.UpdateProgramIdAsync(_studentInfoSystemContext, courseUpdateDTO.ProgramID, existingCourse.ProgramID);

            //Checking if Coursename is provided
            await CourseHelper.UpdateCourseNameAsync(_studentInfoSystemContext, existingCourse, courseUpdateDTO);

            //Checking if CourseCode is provided
            await CourseHelper.UpdateCourseCodeAsync(_studentInfoSystemContext, existingCourse, courseUpdateDTO);

            //Changing description if provided
            if (!string.IsNullOrWhiteSpace(courseUpdateDTO.Description))
                existingCourse.Description = courseUpdateDTO.Description; //Just changing because description validation is not necessary

            //Changing credits
            if (courseUpdateDTO.Credits.HasValue && courseUpdateDTO.Credits != 0)
                existingCourse.Credits = courseUpdateDTO.Credits.Value;

            //Changing ProgramID
            if (courseUpdateDTO.ProgramID.HasValue && courseUpdateDTO.ProgramID!=0)
                existingCourse.ProgramID = courseUpdateDTO.ProgramID.Value;

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
    }
}
