using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;
using StudentInfoSystemApp.Application.DTOs.ScheduleDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Services.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Services.Implementations
{
    public class ScheduleService : IScheduleService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;
        private readonly IPaginationService<Schedule> _paginationService;
        public ScheduleService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper, IPaginationService<Schedule> paginationService)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
            _paginationService = paginationService;
        }

        public async Task<PaginationListDTO<ScheduleReturnDTO>> GetAllAsync(int page = 1, string searchInput = "", int pageSize = 3)
        {
            //Extracting query to not overload requests
            var query = CreateScheduleQuery(searchInput);

            //Applying Pagination logic
            var paginatedSchedules = await _paginationService.ApplyPaginationAsync(query, page, pageSize);

            var totalCount = await query.CountAsync();

            return new PaginationListDTO<ScheduleReturnDTO>
            {
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize=pageSize,
                Objects = _mapper.Map<List<ScheduleReturnDTO>>(paginatedSchedules)
            };
        }
        public async Task<ScheduleReturnDTO> GetByIdAsync(int? id)
        {
            //Validating ID
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");

            //Getting the Schedule from the database
            var schedule = await FindScheduleByIdAsync(id.Value);

            //Not found exception
            if (schedule is null) throw new CustomException(400, "ID", $"Schedule with ID of: '{id}' not found in the database");

            //Returning DTO
            return _mapper.Map<ScheduleReturnDTO>(schedule);
        }
        public async Task<CreateResponseDTO<ScheduleReturnDTO>> CreateAsync(ScheduleCreateDTO scheduleCreateDTO)
        {
            //Validating Course ID
            await ValidateCourseAsync(scheduleCreateDTO.CourseID);

            //Validating Instructor ID
            await ValidateInstructorAsync(scheduleCreateDTO.InstructorID);

            //Checking if Schedule is available or duplicated
            await CheckIfScheduleIsDuplicate(scheduleCreateDTO);

            //Mapping DTO to an object
            Schedule schedule = _mapper.Map<Schedule>(scheduleCreateDTO);

            //Adding the entity to the database
            await _studentInfoSystemContext.AddAsync(schedule);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning the ID of the created entity
            return new CreateResponseDTO<ScheduleReturnDTO>()
            {
                Response = true,
                CreationDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<ScheduleReturnDTO>(schedule)
            };
        }
        public async Task<bool> DeleteAsync(int? id)
        {
            //Checking if requested ID is mentioned
            if (id is null) throw new CustomException(400, "ID", "ID cannot be empty");

            //Checking if a Schedule with requested ID exists in the database
            var existingSchedule = await _studentInfoSystemContext.Schedules.SingleOrDefaultAsync(s => s.ID == id);
            if (existingSchedule == null) throw new CustomException(400, "ID", $"A schedule with ID of: '{id}' not found in the database");

            //Deleting the requested attendance
            _studentInfoSystemContext.Schedules.Remove(existingSchedule);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning true if delete successful
            return true;
        }
        public async Task<UpdateResponseDTO<ScheduleReturnDTO>> UpdateAsync(int? id, ScheduleUpdateDTO scheduleUpdateDTO)
        {
            // Validating ID provided from body
            if (id is null)
                throw new CustomException(400, "Schedule ID", "Schedule ID must not be empty");

            // Find the existing Schedule with ID
            var existingSchedule = await FindScheduleByIdAsync(id.Value);

            if (existingSchedule is null)
                throw new CustomException(400, "Schedule ID", $"A Schedule with ID of: '{id}' not found in the database");

            // Validate CourseID if changed
            if (scheduleUpdateDTO.CourseID != null && existingSchedule.CourseID != scheduleUpdateDTO.CourseID)
            {
                var existingCourse = await _studentInfoSystemContext.Courses.SingleOrDefaultAsync(c => c.ID == scheduleUpdateDTO.CourseID);
                if (existingCourse is null)
                    throw new CustomException(400, "Course ID", $"A Course with ID of: '{scheduleUpdateDTO.CourseID}' not found in the database");
                existingSchedule.CourseID = scheduleUpdateDTO.CourseID.Value;
            }

            // Validate InstructorID if changed
            if (scheduleUpdateDTO.InstructorID != null && existingSchedule.InstructorID != scheduleUpdateDTO.InstructorID)
            {
                var existingInstructor = await _studentInfoSystemContext.Instructors.SingleOrDefaultAsync(i => i.ID == scheduleUpdateDTO.InstructorID);
                if (existingInstructor is null)
                    throw new CustomException(400, "Instructor ID", $"An Instructor with ID of: '{scheduleUpdateDTO.InstructorID}' not found in the database");
                existingSchedule.InstructorID = scheduleUpdateDTO.InstructorID.Value;
            }

            // Update other fields if provided
            if (!string.IsNullOrWhiteSpace(scheduleUpdateDTO.Semester))
                existingSchedule.Semester = scheduleUpdateDTO.Semester.FirstCharToUpper();

            if (!string.IsNullOrWhiteSpace(scheduleUpdateDTO.ClassTime))
                existingSchedule.ClassTime = scheduleUpdateDTO.ClassTime;

            if (!string.IsNullOrWhiteSpace(scheduleUpdateDTO.Classroom))
                existingSchedule.Classroom = scheduleUpdateDTO.Classroom.FirstCharToUpper();

            // Check for overlapping schedules
            var overlappingSchedule = await _studentInfoSystemContext.Schedules
                .FirstOrDefaultAsync(s => s.Semester.Trim().ToLower() == existingSchedule.Semester.Trim().ToLower() &&
                                          s.Classroom.Trim().ToLower() == existingSchedule.Classroom.Trim().ToLower() &&
                                          s.ClassTime == existingSchedule.ClassTime);

            if (overlappingSchedule != null && overlappingSchedule.ID != existingSchedule.ID)
                throw new CustomException(400, "Overlapping Schedules", $"{existingSchedule.Classroom} is busy at {existingSchedule.ClassTime}, on {existingSchedule.Semester}");

            // Save changes to the database
            _studentInfoSystemContext.Schedules.Update(existingSchedule);
            await _studentInfoSystemContext.SaveChangesAsync();

            // Return response DTO
            return new UpdateResponseDTO<ScheduleReturnDTO>
            {
                Response = true,
                UpdateDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<ScheduleReturnDTO>(existingSchedule)
            };
        }



        //Private methods to refactor main methods
        private IQueryable<Schedule> CreateScheduleQuery(string searchInput)
        {
            var query = _studentInfoSystemContext
                .Schedules
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchInput))
            {
                searchInput = searchInput.Trim().ToLower();

                query = query.Where(s =>
                    (s.Semester ?? "").Trim().ToLower().Contains(searchInput) ||
                    (s.ClassTime ?? "").Trim().ToLower().Contains(searchInput) ||
                    (s.Classroom ?? "").Trim().ToLower().Contains(searchInput) ||
                    (s.Course.CourseName ?? "").Trim().ToLower().Contains(searchInput) ||
                    s.Course.CourseCode.Trim().ToLower() == searchInput ||
                    (s.Instructor.FirstName ?? "").Trim().ToLower().Contains(searchInput) ||
                    (s.Instructor.LastName ?? "").Trim().ToLower().Contains(searchInput) ||
                    ((s.Instructor.FirstName ?? "") + " " + (s.Instructor.LastName ?? "")).ToLower().Contains(searchInput)
                );
            }
            return query;
        }
        private async Task<List<Schedule>> ApplyPaginationAsync(IQueryable<Schedule> query, int page, int pageSize)
        {
            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        private async Task<Schedule?> FindScheduleByIdAsync(int id)
        {
            return await _studentInfoSystemContext
                .Schedules
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .FirstOrDefaultAsync(s => s.ID == id);
        }
        private async Task ValidateCourseAsync(int courseId)
        {
            var existingCourse = await _studentInfoSystemContext.Courses.SingleOrDefaultAsync(c => c.ID == courseId);
            if (existingCourse == null)
                throw new CustomException(400, "Course ID", $"A Course with ID of: '{courseId}' not found in the database.");
        }
        private async Task ValidateInstructorAsync(int instructorId)
        {
            // Validate Instructor ID
            var existingInstructor = await _studentInfoSystemContext.Instructors.SingleOrDefaultAsync(i => i.ID == instructorId);
            if (existingInstructor == null)
                throw new CustomException(400, "Instructor ID", $"An Instructor with ID of: '{instructorId}' not found in the database.");
        }
        private async Task CheckIfScheduleIsDuplicate(ScheduleCreateDTO scheduleCreateDTO)
        {
            var existingSchedule = await _studentInfoSystemContext.Schedules.SingleOrDefaultAsync(
                s => s.Semester.Trim().ToLower() == scheduleCreateDTO.Semester.Trim().ToLower() &&
                     s.CourseID == scheduleCreateDTO.CourseID &&
                     s.InstructorID == scheduleCreateDTO.InstructorID);

            if (existingSchedule != null)
                throw new CustomException(400, "Schedule", "A Schedule with the same Semester, Course ID, and Instructor ID already exists in the database.");
        }
    }
}
