using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;
using StudentInfoSystemApp.Application.DTOs.CourseDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Services.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;
using System.Globalization;

namespace StudentInfoSystemApp.Application.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;
        public AttendanceService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
        }

        public async Task<PaginationListDTO<AttendanceReturnDTO>> GetAllAsync(int page = 1, string searchInput = "", int pageSize = 3)
        {
            //Extracting query to not overload requests
            var query = _studentInfoSystemContext.Attendances
                .Include(a => a.Enrollment)
                .ThenInclude(e => e.Student)
                .AsQueryable();

            //Search logic
            if (!string.IsNullOrWhiteSpace(searchInput.Trim().ToLower()))
            {
                var dateFormat = "dd/MM/yyyy";
                if (DateTime.TryParseExact(searchInput.Trim().ToLower(), dateFormat, null, System.Globalization.DateTimeStyles.None, out var searchDate))
                {
                    query = query.Where(a => a.AttendanceDate.Date == searchDate.Date);
                }
                else
                {
                    throw new CustomException(400, "Date Format", "Invalid date format. Please use DD/MM/YYYY.");
                }
            }

            var datas = await query
                .Skip((page - 1) * 2)
                .Take(2)
                .ToListAsync();

            var totalCount = await query.CountAsync();

            return new PaginationListDTO<AttendanceReturnDTO>
            {
                TotalCount = totalCount,
                CurrentPage = page,
                Objects = _mapper.Map<List<AttendanceReturnDTO>>(datas)
            };
        }

        public async Task<AttendanceReturnDTO> GetByIdAsync(int? id)
        {
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");
            var attendance = await _studentInfoSystemContext
                .Attendances
                .Include(a => a.Enrollment)
                .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(a => a.ID == id);
            if (attendance is null) throw new CustomException(400, "ID", $"Attendance with ID of: '{id}' not found in the database");
            return _mapper.Map<AttendanceReturnDTO>(attendance);
        }
        public async Task<int> CreateAsync(AttendanceCreateDTO attendanceCreateDTO)
        {
            //Checking if Attendance already exists
            var existAttendance = await _studentInfoSystemContext.Attendances.SingleOrDefaultAsync(a => a.AttendanceDate.Date == attendanceCreateDTO.AttendanceDate.Date && a.EnrollmentID == attendanceCreateDTO.EnrollmentID);
            if (existAttendance != null)
                throw new CustomException(400, "Attendance", $"Attendance with date of: '{attendanceCreateDTO.AttendanceDate.ToShortDateString()}' and Enrollment ID of: '{attendanceCreateDTO.EnrollmentID}' already exists in the database");

            //Finding relevant Enrollment
            var enrollment = await _studentInfoSystemContext.Enrollments.SingleOrDefaultAsync(e => e.ID == attendanceCreateDTO.EnrollmentID);
            if (enrollment is null)
                throw new CustomException(400, "ID", $"Enrollment with ID of:' {attendanceCreateDTO.EnrollmentID}' not found in the database");

            //Validating AttendanceDate
            AttendanceHelper.ValidateAttendanceDate(attendanceCreateDTO.AttendanceDate);

            //Validating Status
            AttendanceHelper.ValidateStatus(attendanceCreateDTO.Status);

            //Mapping DTO to Object
            Attendance attendance = _mapper.Map<Attendance>(attendanceCreateDTO);

            //Adding entity to the database
            await _studentInfoSystemContext.Attendances.AddAsync(attendance);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning the ID of the created entity
            return attendance.ID;
        }
        public async Task<UpdateResponseDTO<AttendanceReturnDTO>> UpdateAsync(int? id, AttendanceUpdateDTO attendanceUpdateDTO)
        {
            //extracting query
            var query = _studentInfoSystemContext.Attendances
                .Include(a=>a.Enrollment)
                .ThenInclude(ae=>ae.Student)
                .AsQueryable();

            //Checking if ID from body is provided
            if (id is null) throw new CustomException(400, "Attendance ID", "Attendance ID must not be empty");

            //Checking if an attendance with provided ID exists
            var existingAttendance = await query.SingleOrDefaultAsync(a => a.ID == id);
            if (existingAttendance == null) throw new CustomException(400, "Attendance ID", $"An Attendance with ID of: '{id}' not found in the database");

            //Checking if Date is changed
            if (!string.IsNullOrWhiteSpace(attendanceUpdateDTO.AttendanceDate))
            {
                if (DateTime.TryParseExact(attendanceUpdateDTO.AttendanceDate,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime parsedDate))
                {
                    if (parsedDate.Date > DateTime.Now.Date)
                        throw new CustomException(400, "Attendance Date", "Attendance date can not be in the future.");
                    existingAttendance.AttendanceDate = parsedDate;
                }
                else
                {
                    throw new CustomException(400, "Attendance Date", "Invalid date format. Please use dd/MM/yyyy.");
                }
            }

            //Checking if EnrollmentID is changed to validate existence
            if (existingAttendance.EnrollmentID != attendanceUpdateDTO.EnrollmentID)
            {
                attendanceUpdateDTO.EnrollmentID = (attendanceUpdateDTO.EnrollmentID == null || attendanceUpdateDTO.EnrollmentID == 0)
                    ? existingAttendance.EnrollmentID
                    : attendanceUpdateDTO.EnrollmentID;

                // checking if the EnrollmentID has changed and validate it
                if (existingAttendance.EnrollmentID != attendanceUpdateDTO.EnrollmentID)
                {
                    var existingEnrollment = await _studentInfoSystemContext.Enrollments
                        .SingleOrDefaultAsync(e => e.ID == attendanceUpdateDTO.EnrollmentID);

                    if (existingEnrollment is null)
                        throw new CustomException(400, "Enrollment ID", $"An Enrollment with ID of: '{attendanceUpdateDTO.EnrollmentID}' not found in the database");
                }
            }
            //Checking if an Attendance with same date and Enrollment ID already exists
            var duplicateAttendance= await query.SingleOrDefaultAsync(a=>a.AttendanceDate.Date==existingAttendance.AttendanceDate.Date&&a.EnrollmentID==existingAttendance.EnrollmentID);
            if (duplicateAttendance != null&&duplicateAttendance!=existingAttendance)
                throw new CustomException(400, "Duplicate Attendance", "An attendance with same ProgramID and AttendanceDate already exists in the database");
            
            //Checking if Status is changed
            if (existingAttendance.Status != attendanceUpdateDTO.Status)
            {
                //Keeping previous if string is empty or null
                attendanceUpdateDTO.Status = string.IsNullOrWhiteSpace(attendanceUpdateDTO.Status)
                ? existingAttendance.Status
                : attendanceUpdateDTO.Status;

                //Validating if changed
                if (attendanceUpdateDTO.Status != existingAttendance.Status)
                {
                    AttendanceHelper.ValidateStatus(attendanceUpdateDTO.Status);
                }
                    
            }
            
            existingAttendance.Status = attendanceUpdateDTO.Status;
            existingAttendance.EnrollmentID=attendanceUpdateDTO.EnrollmentID.GetValueOrDefault();
            // Save changes
            _studentInfoSystemContext.Update(existingAttendance);
            await _studentInfoSystemContext.SaveChangesAsync();

            return new UpdateResponseDTO<AttendanceReturnDTO>
            {
                Response = true,
                UpdateDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<AttendanceReturnDTO>(existingAttendance)
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
