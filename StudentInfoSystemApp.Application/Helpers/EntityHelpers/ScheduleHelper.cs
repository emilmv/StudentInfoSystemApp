using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.ScheduleDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Helpers.EntityHelpers
{
    public static class ScheduleHelper
    {
        public static async Task<Schedule> GetResponseScheduleAsync(StudentInfoSystemContext context, int id)
        {
            var schedule = await context
                .Schedules
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .FirstOrDefaultAsync(c => c.ID == id);

            if (schedule == null)
                throw new CustomException(400, "ScheduleID", $"A schedule with ID of: '{id}' not found in the database.");

            return schedule;
        }

        public static async Task<Schedule> GetScheduleByIdAsync(StudentInfoSystemContext context, int id)
        {
            var schedule = await context.Schedules
                .FirstOrDefaultAsync(p => p.ID == id);

            if (schedule == null)
                throw new CustomException(400, "Schedule ID", $"A schedule with ID of: '{id}' not found in the database");

            return schedule;
        }

        public static async Task<IQueryable<Schedule>> CreateScheduleQueryAsync(StudentInfoSystemContext context, string searchInput)
        {
            var query = context
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
            var results = await query.ToListAsync();

            if (!results.Any())
                throw new CustomException(404, "Nothing found", "No records were found matching the search criteria.");

            return query;
        }

        public static async Task ValidateCourseAsync(StudentInfoSystemContext context, int courseId)
        {
            var existingCourse = await context.Courses.SingleOrDefaultAsync(c => c.ID == courseId);
            if (existingCourse == null)
                throw new CustomException(400, "Course ID", $"A Course with ID of: '{courseId}' not found in the database.");
        }

        public static async Task ValidateInstructorAsync(StudentInfoSystemContext context, int instructorId)
        {
            // Validate Instructor ID
            var existingInstructor = await context.Instructors.SingleOrDefaultAsync(i => i.ID == instructorId);
            if (existingInstructor == null)
                throw new CustomException(400, "Instructor ID", $"An Instructor with ID of: '{instructorId}' not found in the database.");
        }

        public static async Task CheckIfScheduleIsDuplicate(StudentInfoSystemContext context, ScheduleCreateDTO scheduleCreateDTO)
        {
            var existingSchedule = await context.Schedules.FirstOrDefaultAsync(
                s => s.Semester.Trim().ToLower() == scheduleCreateDTO.Semester.Trim().ToLower() &&
                     s.CourseID == scheduleCreateDTO.CourseID &&
                     s.InstructorID == scheduleCreateDTO.InstructorID);

            if (existingSchedule != null)
                throw new CustomException(400, "Schedule", "A Schedule with the same Semester, Course ID, and Instructor ID already exists in the database.");
        }

        public static async Task CheckForOverlappingSchedules(StudentInfoSystemContext context, Schedule existingSchedule)
        {
            var overlappingSchedule = await context.Schedules
                 .FirstOrDefaultAsync(s => s.Semester.Trim().ToLower() == existingSchedule.Semester.Trim().ToLower() &&
                                           s.Classroom.Trim().ToLower() == existingSchedule.Classroom.Trim().ToLower() &&
                                           s.ClassTime == existingSchedule.ClassTime);

            if (overlappingSchedule != null && overlappingSchedule.ID != existingSchedule.ID)
                throw new CustomException(400, "Overlapping Schedules", $"{existingSchedule.Classroom} is busy at {existingSchedule.ClassTime}, on {existingSchedule.Semester}");
        }

        public static async Task UpdateInstructorIdAsync(StudentInfoSystemContext context, Schedule existingSchedule, ScheduleUpdateDTO scheduleUpdateDTO)
        {
            if (scheduleUpdateDTO.InstructorID != null && existingSchedule.InstructorID != scheduleUpdateDTO.InstructorID)
            {
                var existingInstructor = await context.Instructors.SingleOrDefaultAsync(i => i.ID == scheduleUpdateDTO.InstructorID);
                if (existingInstructor is null)
                    throw new CustomException(400, "Instructor ID", $"An Instructor with ID of: '{scheduleUpdateDTO.InstructorID}' not found in the database");
                existingSchedule.InstructorID = scheduleUpdateDTO.InstructorID.Value;
            }
        }

        public static async Task UpdateCourseIdAsync(StudentInfoSystemContext context, Schedule existingSchedule, ScheduleUpdateDTO scheduleUpdateDTO)
        {
            if (scheduleUpdateDTO.CourseID != null && existingSchedule.CourseID != scheduleUpdateDTO.CourseID)
            {
                var existingCourse = await context.Courses.SingleOrDefaultAsync(c => c.ID == scheduleUpdateDTO.CourseID);
                if (existingCourse is null)
                    throw new CustomException(400, "Course ID", $"A Course with ID of: '{scheduleUpdateDTO.CourseID}' not found in the database");
                existingSchedule.CourseID = scheduleUpdateDTO.CourseID.Value;
            }
        }

        public static async Task CheckIfClassroomIsFreeToCreateAsync(StudentInfoSystemContext context, ScheduleCreateDTO scheduleCreateDTO)
        {
            var busyClassroom = await context.Schedules
                .FirstOrDefaultAsync(s => s.Semester.Trim().ToLower() == scheduleCreateDTO.Semester.Trim().ToLower() &&
                                          s.Classroom.Trim().ToLower() == scheduleCreateDTO.Classroom.Trim().ToLower() &&
                                          s.ClassTime == scheduleCreateDTO.ClassTime);
            if (busyClassroom != null)
                throw new CustomException(400, "Busy Classroom", $"Room: {scheduleCreateDTO.Classroom} is busy at {scheduleCreateDTO.ClassTime} on {scheduleCreateDTO.Semester}");
        }

        public static async Task CheckIfInstructorIsFreeToCreateAsync(StudentInfoSystemContext context, ScheduleCreateDTO scheduleCreateDTO)
        {
            var busyInstructor = await context.Schedules
                .Include(s=>s.Instructor)
                .FirstOrDefaultAsync(s => s.Semester.Trim().ToLower() == scheduleCreateDTO.Semester.Trim().ToLower() &&
                                          s.ClassTime == scheduleCreateDTO.ClassTime&&
                                          s.InstructorID==scheduleCreateDTO.InstructorID);
            if (busyInstructor != null)
                throw new CustomException(400, "Busy Instructor", $"Mr./Mrs. {busyInstructor.Instructor.FirstName + " " + busyInstructor.Instructor.LastName} is already registered to a class in {scheduleCreateDTO.Semester} at {scheduleCreateDTO.ClassTime}.");
        }

        public static async Task CheckIfInstructorIsFreeToUpdateAsync(StudentInfoSystemContext context, ScheduleUpdateDTO scheduleUpdateDTO)
        {
            var busyInstructor = await context.Schedules
                .Include(s => s.Instructor)
                .FirstOrDefaultAsync(s => s.Semester.Trim().ToLower() == scheduleUpdateDTO.Semester.Trim().ToLower() &&
                                          s.ClassTime == scheduleUpdateDTO.ClassTime &&
                                          s.InstructorID == scheduleUpdateDTO.InstructorID);
            if (busyInstructor != null)
                throw new CustomException(400, "Busy Instructor", $"Mr./Mrs. {busyInstructor.Instructor.FirstName + " " + busyInstructor.Instructor.LastName} is already registered to a class in {scheduleUpdateDTO.Semester} at {scheduleUpdateDTO.ClassTime}.");
        }

        public static async Task CheckIfClassroomIsFreeToUpdateAsync(StudentInfoSystemContext context, ScheduleUpdateDTO scheduleUpdateDTO)
        {
            var busyClassroom = await context.Schedules
                .FirstOrDefaultAsync(s => s.Semester.Trim().ToLower() == scheduleUpdateDTO.Semester.Trim().ToLower() &&
                                          s.Classroom.Trim().ToLower() == scheduleUpdateDTO.Classroom.Trim().ToLower() &&
                                          s.ClassTime == scheduleUpdateDTO.ClassTime);
            if (busyClassroom != null)
                throw new CustomException(400, "Busy Classroom", $"Room: {scheduleUpdateDTO.Classroom} is busy at {scheduleUpdateDTO.ClassTime} on {scheduleUpdateDTO.Semester}");
        }
    }
}
