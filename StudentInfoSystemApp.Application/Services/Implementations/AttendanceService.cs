using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Services.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;
        private readonly IPaginationService<Attendance> _paginationService;
        public AttendanceService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper, IPaginationService<Attendance> paginationService)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
            _paginationService = paginationService;
        }

        public async Task<PaginationListDTO<AttendanceReturnDTO>> GetAllAsync(int page = 1, string searchInput = "", int pageSize = 3)
        {
            //Extracting query to not overload requests
            var query =await AttendanceHelper.CreateAttendanceQueryAsync(_studentInfoSystemContext,searchInput);

            //Pagination
            var pagedAttendanceDatas = await _paginationService.ApplyPaginationAsync(query, page, pageSize);

            var totalCount = await query.CountAsync();

            return new PaginationListDTO<AttendanceReturnDTO>
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
                Objects = _mapper.Map<List<AttendanceReturnDTO>>(pagedAttendanceDatas)
            };
        }
        public async Task<AttendanceReturnDTO> GetByIdAsync(int? id)
        {
            //Validating ID
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");

            //Getting the Attendance
            var ResponseAttendance = await AttendanceHelper.GetResponseAttendanceAsync(_studentInfoSystemContext, id.Value);

            //Returning DTO
            return _mapper.Map<AttendanceReturnDTO>(ResponseAttendance);
        }
        public async Task<CreateResponseDTO<AttendanceReturnDTO>> CreateAsync(AttendanceCreateDTO attendanceCreateDTO)
        {
            //Checking if Attendance is duplicate
            await AttendanceHelper.EnsureAttendanceDoesNotExistAsync(_studentInfoSystemContext,attendanceCreateDTO.AttendanceDate, attendanceCreateDTO.EnrollmentID);

            //Finding relevant Enrollment
            await AttendanceHelper.EnsureEnrollmentExistsAsync(_studentInfoSystemContext,attendanceCreateDTO.EnrollmentID);

            //Validating AttendanceDate
            AttendanceHelper.ValidateAttendanceDate(attendanceCreateDTO.AttendanceDate);

            //Validating Status
            AttendanceHelper.ValidateStatus(attendanceCreateDTO.Status);

            //Mapping DTO to Object
            Attendance attendance = _mapper.Map<Attendance>(attendanceCreateDTO);

            //Adding entity to the database
            await _studentInfoSystemContext.Attendances.AddAsync(attendance);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Getting responseDTO
            var responseAttendance = await AttendanceHelper.GetResponseAttendanceAsync(_studentInfoSystemContext,attendance.ID);

            //Returning the ID of the created entity
            return new CreateResponseDTO<AttendanceReturnDTO>()
            {
                Response = true,
                CreationDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<AttendanceReturnDTO>(responseAttendance)
            };
        }
        public async Task<UpdateResponseDTO<AttendanceReturnDTO>> UpdateAsync(int? id, AttendanceUpdateDTO attendanceUpdateDTO)
        {
            //Validating ID
            if (id is null) throw new CustomException(400, "Attendance ID", "Attendance ID must not be empty");

            //Checking if an attendance with provided ID exists
            var existingAttendance = await AttendanceHelper.GetAttendanceByIdAsync(_studentInfoSystemContext,id.Value);

            //Checking if Date is changed
            AttendanceHelper.UpdateAttendanceDate(existingAttendance, attendanceUpdateDTO);

            //Checking if EnrollmentID is changed to validate existence
            await AttendanceHelper.UpdateEnrollmentIdAsync(_studentInfoSystemContext,existingAttendance, attendanceUpdateDTO);

            //Checking if an Attendance with same date and Enrollment ID already exists
            await AttendanceHelper.EnsureAttendanceIsNotDuplicateAsync(_studentInfoSystemContext,existingAttendance, attendanceUpdateDTO);

            //Checking if Status is changed
            AttendanceHelper.UpdateStatus(existingAttendance, attendanceUpdateDTO);

            //Updating fields
            existingAttendance.Status = attendanceUpdateDTO.Status.FirstCharToUpper();
            existingAttendance.EnrollmentID = attendanceUpdateDTO.EnrollmentID.GetValueOrDefault();

            // Save changes
            _studentInfoSystemContext.Update(existingAttendance);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Getting responseDTO
            var responseAttendance = await AttendanceHelper.GetResponseAttendanceAsync(_studentInfoSystemContext, existingAttendance.ID);

            return new UpdateResponseDTO<AttendanceReturnDTO>
            {
                Response = true,
                UpdateDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<AttendanceReturnDTO>(responseAttendance)
            };
        }
        public async Task<bool> DeleteAsync(int? id)
        {
            //Checking if requested ID is mentioned
            if (id is null) throw new CustomException(400, "ID", "ID cannot be empty");

            //Checking if an Attendance with requested ID exists in the database
            var existingAttendance = _studentInfoSystemContext.Attendances.SingleOrDefault(a => a.ID == id);
            if (existingAttendance == null) throw new CustomException(400, "ID", $"An attendance with ID of: '{id}' not found in the database");

            //Deleting the requested attendance
            _studentInfoSystemContext.Attendances.Remove(existingAttendance);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning true if delete successful
            return true;
        }
    }
}
