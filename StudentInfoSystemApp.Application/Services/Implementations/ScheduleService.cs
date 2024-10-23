using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;
using StudentInfoSystemApp.Application.DTOs.ScheduleDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Helpers.EntityHelpers;
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
            var query = await ScheduleHelper.CreateScheduleQueryAsync(_studentInfoSystemContext, searchInput);

            //Applying Pagination logic
            var paginatedSchedules = await _paginationService.ApplyPaginationAsync(query, page, pageSize);

            var totalCount = await query.CountAsync();

            return new PaginationListDTO<ScheduleReturnDTO>
            {
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize,
                Objects = _mapper.Map<List<ScheduleReturnDTO>>(paginatedSchedules)
            };
        }
        public async Task<ScheduleReturnDTO> GetByIdAsync(int? id)
        {
            //Validating ID
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");

            //Getting the Schedule from the database
            var schedule = await ScheduleHelper.GetResponseScheduleAsync(_studentInfoSystemContext, id.Value);

            //Returning DTO
            return _mapper.Map<ScheduleReturnDTO>(schedule);
        }
        public async Task<CreateResponseDTO<ScheduleReturnDTO>> CreateAsync(ScheduleCreateDTO scheduleCreateDTO)
        {
            //Validating Course ID
            await ScheduleHelper.ValidateCourseAsync(_studentInfoSystemContext, scheduleCreateDTO.CourseID);

            //Validating Instructor ID
            await ScheduleHelper.ValidateInstructorAsync(_studentInfoSystemContext, scheduleCreateDTO.InstructorID);

            //Checking if Schedule is available or duplicated
            await ScheduleHelper.CheckIfScheduleIsDuplicate(_studentInfoSystemContext, scheduleCreateDTO);

            //Check if classrom is free
            await ScheduleHelper.CheckIfClassroomIsFreeToCreateAsync(_studentInfoSystemContext, scheduleCreateDTO);

            //Check if Instructor is free
            await ScheduleHelper.CheckIfInstructorIsFreeToCreateAsync(_studentInfoSystemContext,scheduleCreateDTO);

            //Mapping DTO to an object
            Schedule schedule = _mapper.Map<Schedule>(scheduleCreateDTO);

            //Adding the entity to the database
            await _studentInfoSystemContext.AddAsync(schedule);
            await _studentInfoSystemContext.SaveChangesAsync();

            //ResponseDTO
            var responseSchedule = await ScheduleHelper.GetResponseScheduleAsync(_studentInfoSystemContext, schedule.ID);

            //Returning ResponseDTO of the created entity
            return new CreateResponseDTO<ScheduleReturnDTO>()
            {
                Response = true,
                CreationDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<ScheduleReturnDTO>(responseSchedule)
            };
        }
        public async Task<bool> DeleteAsync(int? id)
        {
            //Checking if requested ID is mentioned
            if (id is null) throw new CustomException(400, "ID", "ID cannot be empty");

            //Deleting the requested attendance
            _studentInfoSystemContext.Schedules.Remove(await ScheduleHelper.GetScheduleByIdAsync(_studentInfoSystemContext,id.Value));
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
            var existingSchedule = await ScheduleHelper.GetResponseScheduleAsync(_studentInfoSystemContext, id.Value);

            // Validate CourseID if changed
            await ScheduleHelper.UpdateCourseIdAsync(_studentInfoSystemContext,existingSchedule, scheduleUpdateDTO);
            
            // Validate InstructorID if changed
            await ScheduleHelper.UpdateInstructorIdAsync(_studentInfoSystemContext, existingSchedule, scheduleUpdateDTO);

            // Update other fields if provided
            if (!string.IsNullOrWhiteSpace(scheduleUpdateDTO.Semester))
                existingSchedule.Semester = scheduleUpdateDTO.Semester.FirstCharToUpper();

            if (!string.IsNullOrWhiteSpace(scheduleUpdateDTO.ClassTime))
                existingSchedule.ClassTime = scheduleUpdateDTO.ClassTime;

            if (!string.IsNullOrWhiteSpace(scheduleUpdateDTO.Classroom))
                existingSchedule.Classroom = scheduleUpdateDTO.Classroom.FirstCharToUpper();

            //Check if Instructor is free
            await ScheduleHelper.CheckIfInstructorIsFreeToUpdateAsync(_studentInfoSystemContext, scheduleUpdateDTO);

            //Check if Classroom is free
            await ScheduleHelper.CheckIfClassroomIsFreeToUpdateAsync(_studentInfoSystemContext,scheduleUpdateDTO);

            // Check for overlapping schedules
            await ScheduleHelper.CheckForOverlappingSchedules(_studentInfoSystemContext,existingSchedule);
            
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
    }
}
