using StudentInfoSystemApp.Application.Exceptions;

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
}