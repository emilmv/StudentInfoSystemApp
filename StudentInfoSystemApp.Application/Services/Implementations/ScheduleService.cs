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
        public ScheduleService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
        }

        public async Task<PaginationListDTO<ScheduleReturnDTO>> GetAllAsync(int page = 1, string searchInput = "")
        {
            //Extracting query to not overload requests
            var query = _studentInfoSystemContext
                .Schedules
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .AsQueryable();

            //Search logic
            if (!string.IsNullOrWhiteSpace(searchInput))
            {
                searchInput = searchInput.Trim().ToLower();

                query = query.Where(s =>
                    s.Semester != null && s.Semester.Trim().ToLower().Contains(searchInput) ||
                    s.ClassTime != null && s.ClassTime.Trim().ToLower().Contains(searchInput) ||
                    s.Classroom != null && s.Classroom.Trim().ToLower().Contains(searchInput) ||
                    s.Course.CourseName != null && s.Course.CourseName.Trim().ToLower().Contains(searchInput) ||
                    s.Course.CourseCode.Trim().ToLower().ToString() == searchInput ||
                    s.Instructor.FirstName != null && s.Instructor.FirstName.Trim().ToLower().Contains(searchInput) ||
                    s.Instructor.LastName != null && s.Instructor.LastName.Trim().ToLower().Contains(searchInput) ||
                    ((s.Instructor.FirstName ?? string.Empty) + " " + (s.Instructor.LastName ?? string.Empty)).ToLower().Contains(searchInput)
                );
            }

            var datas = await query
               .Skip((page - 1) * 2)
               .Take(2)
               .ToListAsync();

            var totalCount = await query.CountAsync();

            return new PaginationListDTO<ScheduleReturnDTO>
            {
                TotalCount = totalCount,
                CurrentPage = page,
                Objects = _mapper.Map<List<ScheduleReturnDTO>>(datas)
            };
        }

        public async Task<ScheduleReturnDTO> GetByIdAsync(int? id)
        {
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");
            var schedule = await _studentInfoSystemContext
                .Schedules
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .FirstOrDefaultAsync(d => d.ID == id);
            if (schedule is null) throw new CustomException(400, "ID", $"Schedule with ID of:'{id}'not found in the database");
            return _mapper.Map<ScheduleReturnDTO>(schedule);
        }

        public async Task<int> CreateAsync(ScheduleCreateDTO scheduleCreateDTO)
        {
            //Validating Course ID
            var existingCourse = await _studentInfoSystemContext.Courses.SingleOrDefaultAsync(c => c.ID == scheduleCreateDTO.CourseID);
            if (existingCourse == null) throw new CustomException(400, "Course ID", $"A Course with ID of: '{scheduleCreateDTO.CourseID}' not found in the database.");

            //Validating Instructor ID
            var existingInstructor = await _studentInfoSystemContext.Instructors.SingleOrDefaultAsync(i => i.ID == scheduleCreateDTO.InstructorID);
            if (existingInstructor == null) throw new CustomException(400, "Instructor ID", $"An Instructor with ID of: '{scheduleCreateDTO.InstructorID}' not found in the database");

            //Checking if Schedule exists in the database
            var existingSchedule = await _studentInfoSystemContext.Schedules.SingleOrDefaultAsync(
                s => s.Semester.Trim().ToLower() == scheduleCreateDTO.Semester.Trim().ToLower() &&
                s.CourseID == scheduleCreateDTO.CourseID &&
                s.InstructorID == scheduleCreateDTO.InstructorID);
            if (existingSchedule != null) throw new CustomException(400, "Schedule", $"A Schedule with same Semester, Course ID and Instructor ID already exists in the database.");

            //Mapping DTO to an object
            Schedule schedule = _mapper.Map<Schedule>(scheduleCreateDTO);

            //Adding the entity to the database
            await _studentInfoSystemContext.AddAsync(schedule);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning the ID of the created entity
            return schedule.ID;
        }
        public async Task<bool> DeleteAsync(int? id)
        {
            //Checking if requested ID is mentioned
            if (id is null) throw new CustomException(400, "ID", "ID cannot be empty");

            //Checking if a Schedule with requested ID exists in the database
            var existingSchedule = _studentInfoSystemContext.Schedules.SingleOrDefault(a => a.ID == id);
            if (existingSchedule == null) throw new CustomException(400, "ID", $"A schedule with ID of: '{id}' not found in the database");

            //Deleting the requested attendance
            _studentInfoSystemContext.Schedules.Remove(existingSchedule);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning true if delete successful
            return true;
        }
        public async Task<UpdateResponseDTO<ScheduleReturnDTO>> UpdateAsync(int? id, ScheduleUpdateDTO scheduleUpdateDTO)
        {
            //extracting query
            var query = _studentInfoSystemContext
               .Schedules
               .Include(s => s.Course)
               .Include(s => s.Instructor)
               .AsQueryable();

            //Checking if ID from body is provided
            if (id is null) throw new CustomException(400, "Schedule ID", "Schedule ID must not be empty");

            //Finding relevant Schedule with ID
            var existingSchedule = await query.FirstOrDefaultAsync(s => s.ID == id);
            if (existingSchedule == null) throw new CustomException(400, "Schedule ID", $"A Schedule with ID of: '{id}' not found in the database");

            //Checking if CourseID is changed to validate existence
            if (existingSchedule.CourseID != scheduleUpdateDTO.CourseID)
            {
                scheduleUpdateDTO.CourseID = (scheduleUpdateDTO.CourseID == null || scheduleUpdateDTO.CourseID == 0)
                    ? existingSchedule.CourseID
                    : scheduleUpdateDTO.CourseID;

                // checking if provided
                if (existingSchedule.CourseID != scheduleUpdateDTO.CourseID)
                {
                    //Finding the course
                    var existingCourse = await _studentInfoSystemContext.Courses.SingleOrDefaultAsync(c => c.ID == scheduleUpdateDTO.CourseID);
                    if (existingCourse is null) throw new CustomException(400, "Course ID", $"A Course with ID of: '{scheduleUpdateDTO.CourseID}' not found in the database");
                }
            }
            //Same thing for Instructor
            if (existingSchedule.InstructorID != scheduleUpdateDTO.InstructorID)
            {
                //Checking if null or 0 to keep previous, change if provided
                scheduleUpdateDTO.InstructorID = (scheduleUpdateDTO.InstructorID == null || scheduleUpdateDTO.InstructorID == 0)
                    ? existingSchedule.InstructorID
                    : scheduleUpdateDTO.InstructorID;

                //checking if provided
                if (existingSchedule.InstructorID != scheduleUpdateDTO.InstructorID)
                {
                    //Finding the Instructor
                    var existingInstructor = await _studentInfoSystemContext.Instructors.SingleOrDefaultAsync(s => s.ID == scheduleUpdateDTO.InstructorID);
                    if (existingInstructor is null) throw new CustomException(400, "Instructor ID", $"An Instructor with ID of: '{scheduleUpdateDTO.InstructorID}' not found in the database");
                }
            }
            //Other fields
            if (!string.IsNullOrWhiteSpace(scheduleUpdateDTO.Semester))
                existingSchedule.Semester = scheduleUpdateDTO.Semester.FirstCharToUpper();
            if (!string.IsNullOrWhiteSpace(scheduleUpdateDTO.ClassTime))
                existingSchedule.ClassTime = scheduleUpdateDTO.ClassTime;
            if (!string.IsNullOrWhiteSpace(scheduleUpdateDTO.Classroom))
                existingSchedule.Classroom = scheduleUpdateDTO.Classroom.FirstCharToUpper();

            //checking if classes are not busy
            var overlappingSchedule = await _studentInfoSystemContext.Schedules
                .FirstOrDefaultAsync(s => s.Semester.Trim().ToLower() == existingSchedule.Semester.Trim().ToLower() && //semester check
                             s.Classroom.Trim().ToLower() == existingSchedule.Classroom.Trim().ToLower() && //classroom check
                             s.ClassTime.Trim() == existingSchedule.ClassTime.Trim()); //classtime check
            if (overlappingSchedule!=null&&overlappingSchedule!=existingSchedule) throw new CustomException(400, "Overlapping Schedules", $"{existingSchedule.Classroom} is busy at {existingSchedule.ClassTime}, on {existingSchedule.Semester}");

            //Save changes
            _studentInfoSystemContext.Update(existingSchedule);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Return response DTO
            return new UpdateResponseDTO<ScheduleReturnDTO>()
            {
                Response=true,
                UpdateDate=DateTime.Now.ToShortDateString(),
                Objects=_mapper.Map<ScheduleReturnDTO>(existingSchedule)
            };
        }
    }
}
