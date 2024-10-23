using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;
using System.Globalization;

public static class AttendanceHelper
{
    public static void ValidateStatus(string status)
    {
        List<string> validStatuses = new List<string> { "Absent", "Present" };

        if (!validStatuses.Any(s => string.Equals(s, status, StringComparison.OrdinalIgnoreCase)))
        {
            throw new CustomException(400, "Status", "Status must be either 'Absent' or 'Present'.");
        }
    }

    public static void ValidateAttendanceDate(DateTime? attendanceDate)
    {
        var currentDate = DateTime.UtcNow;
        var maxAllowedDate = currentDate.AddDays(-7);

        if (attendanceDate < maxAllowedDate)
        {
            throw new CustomException(400, "Attendance Date", "Attendance can only be recorded within the past 7 days.");
        }
    }

    public static async Task EnsureAttendanceDoesNotExistAsync(StudentInfoSystemContext context, DateTime attendanceDate, int enrollmentID)
    {
        var existingAttendance = await context.Attendances.SingleOrDefaultAsync(a => a.AttendanceDate.Date == attendanceDate.Date && a.EnrollmentID == enrollmentID);
        if (existingAttendance != null)
            throw new CustomException(400, "Attendance", $"Attendance with date of: '{attendanceDate.ToShortDateString()}' and Enrollment ID of: '{enrollmentID}' already exists in the database");
    }

    public static async Task EnsureEnrollmentExistsAsync(StudentInfoSystemContext context, int enrollmentID)
    {
        var enrollment = await context.Enrollments.SingleOrDefaultAsync(e => e.ID == enrollmentID);
        if (enrollment is null)
            throw new CustomException(400, "ID", $"Enrollment with ID of:' {enrollmentID}' not found in the database");
    }

    public static async Task<Attendance> GetResponseAttendanceAsync(StudentInfoSystemContext context, int id)
    {
        var attendance = await context
                .Attendances
                .Include(a => a.Enrollment)
                .ThenInclude(ae => ae.Student)
                .Include(a => a.Enrollment)
                .ThenInclude(ae => ae.Course)
                .SingleOrDefaultAsync(a => a.ID == id);

        if (attendance is null)
            throw new CustomException(400, "Attendance ID", $"An attendance with ID of: '{id}' not found in the database.");

        return attendance;
    }

    public static void UpdateAttendanceDate(Attendance existingAttendance, AttendanceUpdateDTO attendanceUpdateDTO)
    {
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
                if (parsedDate.Date < DateTime.Now.AddDays(-7))
                    throw new CustomException(400, "Attendance Date", "Attendance can only be recorded within the past 7 days.");
                existingAttendance.AttendanceDate = parsedDate;
            }
            else
                throw new CustomException(400, "Attendance Date", "Invalid date format. Please use dd/MM/yyyy.");
        }
    }

    public static async Task UpdateEnrollmentIdAsync(StudentInfoSystemContext context, Attendance existingAttendance, AttendanceUpdateDTO attendanceUpdateDTO)
    {
        if (existingAttendance.EnrollmentID != attendanceUpdateDTO.EnrollmentID)
        {
            attendanceUpdateDTO.EnrollmentID = (attendanceUpdateDTO.EnrollmentID == null || attendanceUpdateDTO.EnrollmentID == 0)
                ? existingAttendance.EnrollmentID
                : attendanceUpdateDTO.EnrollmentID;

            // checking if the EnrollmentID has changed and validate it
            if (existingAttendance.EnrollmentID != attendanceUpdateDTO.EnrollmentID)
            {
                var existingEnrollment = await context.Enrollments
                    .SingleOrDefaultAsync(e => e.ID == attendanceUpdateDTO.EnrollmentID);

                if (existingEnrollment is null)
                    throw new CustomException(400, "Enrollment ID", $"An Enrollment with ID of: '{attendanceUpdateDTO.EnrollmentID}' not found in the database");
            }
        }
    }

    public static async Task EnsureAttendanceIsNotDuplicateAsync(StudentInfoSystemContext context, Attendance existingAttendance, AttendanceUpdateDTO attendanceUpdateDTO)
    {
        var duplicateAttendance = await context.Attendances.SingleOrDefaultAsync(a => a.AttendanceDate.Date == existingAttendance.AttendanceDate.Date && a.EnrollmentID == existingAttendance.EnrollmentID);
        if (duplicateAttendance != null && duplicateAttendance.ID != existingAttendance.ID)
            throw new CustomException(400, "Duplicate Attendance", "An attendance with same ProgramID and AttendanceDate already exists in the database");
    }

    public static void UpdateStatus(Attendance existingAttendance, AttendanceUpdateDTO attendanceUpdateDTO)
    {
        if (existingAttendance.Status != attendanceUpdateDTO.Status)
        {
            //Keeping previous if string is empty or null
            attendanceUpdateDTO.Status = string.IsNullOrWhiteSpace(attendanceUpdateDTO.Status)
            ? existingAttendance.Status
            : attendanceUpdateDTO.Status;

            //Validating if changed
            if (attendanceUpdateDTO.Status != existingAttendance.Status)
                AttendanceHelper.ValidateStatus(attendanceUpdateDTO.Status);
        }
    }

    public static async Task<Attendance> GetAttendanceByIdAsync(StudentInfoSystemContext context, int id)
    {
        var attendance = await context
            .Attendances
            .FirstOrDefaultAsync(a => a.ID == id);

        if (attendance is null) throw new CustomException(400, "ID", $"Attendance with ID of: '{id}' not found in the database");

        return attendance;
    }

    public static async Task<IQueryable<Attendance>> CreateAttendanceQueryAsync(StudentInfoSystemContext context, string searchInput)
    {
        //Base query
        var query = context
            .Attendances
            .Include(a => a.Enrollment)
            .ThenInclude(ae => ae.Student)
            .Include(a => a.Enrollment)
            .ThenInclude(ae => ae.Course)
            .AsQueryable();

        //Search logic
        if (!string.IsNullOrWhiteSpace(searchInput))
        {
            if (DateTime.TryParseExact(searchInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var searchDate))
                query = query.Where(e => e.AttendanceDate.Date == searchDate.Date);
            else
            {
                searchInput = searchInput.Trim().ToLower();
                query = query.Where(s =>
                    (s.Enrollment.Student.FirstName ?? "").ToLower().Contains(searchInput) ||
                    (s.Enrollment.Student.LastName ?? "").ToLower().Contains(searchInput) ||
                    ((s.Enrollment.Student.FirstName ?? "") + " " + (s.Enrollment.Student.LastName ?? "")).ToLower().Contains(searchInput));
            }
        }
        var results = await query.ToListAsync();
        if (!results.Any())
            throw new CustomException(404, "Nothing found", "No records were found matching the search criteria.");
        return query;
    }
}