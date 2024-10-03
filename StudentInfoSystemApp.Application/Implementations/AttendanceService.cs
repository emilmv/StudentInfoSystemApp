using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Implementations
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

        public async Task<AttendanceListDTO> GetAllAsync(int page=1,string searchInput ="")
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
                    throw new CustomException(400,"Date Format","Invalid date format. Please use DD/MM/YYYY.");
                }
            }

            var datas=await query
                .Skip((page - 1) *2)
                .Take(2)
                .ToListAsync();

            var totalCount=await query.CountAsync();

            AttendanceListDTO AttendanceListDTO = new();
            AttendanceListDTO.TotalCount = totalCount;
            AttendanceListDTO.CurrentPage = page;
            AttendanceListDTO.Attendances = _mapper.Map<List<AttendanceReturnDTO>>(datas);

            return AttendanceListDTO;
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
            var currentDate = DateTime.UtcNow;
            var maxAllowedDate = currentDate.AddDays(-7);
            if (attendanceCreateDTO.AttendanceDate < maxAllowedDate)
                throw new CustomException(400, "Attendance Date", "Attendance can only be recorded within the past 7 days.");

            //Validating Status
            var validStatuses = new List<string> { "Absent", "Present" };
            if (!validStatuses.Any(s => string.Equals(s, attendanceCreateDTO.Status, StringComparison.OrdinalIgnoreCase)))
                throw new CustomException(400, "Status", "Status must be either 'Absent' or 'Present'.");

            //Mapping DTO to Object
            Attendance attendance = _mapper.Map<Attendance>(attendanceCreateDTO);

            //Adding entity to the database
            await _studentInfoSystemContext.Attendances.AddAsync(attendance);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning the ID of the created entity
            return attendance.ID;
        }
    }
}
