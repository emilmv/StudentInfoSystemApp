using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.CourseDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Helpers.EntityHelpers
{
    public static class CourseHelper
    {
        public static async Task<IQueryable<Course>> CreateCourseQueryAsync(StudentInfoSystemContext context, string searchInput)
        {
            var query = context.Courses
                .Include(c => c.Program)
                .Include(c => c.Enrollments)
                .Include(c => c.Schedules)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchInput))
            {
                query = query.Where(c =>
                    c.CourseName.ToLower().Contains(searchInput.ToLower()) ||
                    c.CourseCode.ToLower().Contains(searchInput.ToLower()) ||
                    c.Description.ToLower().Contains(searchInput.ToLower())
                );
            }

            var results = await query.ToListAsync();

            if (!results.Any())
                throw new CustomException(404, "Nothing found", "No records were found matching the search criteria.");
            return query;
        }
        
        public static async Task<Course> GetResponseCourseAsync(StudentInfoSystemContext context,int id)
        {
            var course = await context
               .Courses
               .Include(c => c.Program)
               .Include(c => c.Enrollments)
               .Include(c => c.Schedules)
               .FirstOrDefaultAsync(c => c.ID == id);

            if (course is null) throw new CustomException(400, "ID", $"A Course with ID of:'{id}'not found in the database");

            return course;
        }

        public static async Task EnsureCourseDoesNotExistAsync(StudentInfoSystemContext context,string courseName)
        {
            var existCourse = await context.Courses.SingleOrDefaultAsync(c => c.CourseName.ToLower() == courseName.ToLower());
            if (existCourse != null)
                throw new CustomException(400, "CourseName", $"Course with name of: '{courseName}' already exists in the database.");
        }

        public static async Task CheckCourseCodeUniquenessAsync(StudentInfoSystemContext context,string courseCode)
        {
            var existingCourseCode = await context.Courses.SingleOrDefaultAsync(c => c.CourseCode.Trim().ToLower() == courseCode.Trim().ToLower());
            if (existingCourseCode != null)
                throw new CustomException(400, "CourseCode", $"A course with code of: '{courseCode}' already exists in the database");
        }

        public static async Task EnsureProgramExistsAsync(StudentInfoSystemContext context,int programID)
        {
            var program = await context.Programs.SingleOrDefaultAsync(e => e.ID == programID);
            if (program is null)
                throw new CustomException(400, "ID", $"Program with ID of:' {programID}' not found in the database");
        }

        public static async Task<Course>FindCourseByID(StudentInfoSystemContext context,int courseID)
        {
            var existingCourse = await context.Courses.SingleOrDefaultAsync(c => c.ID == courseID);
            if (existingCourse is null) throw new CustomException(400, "ID", $"A Course with ID of: '{courseID}' not found in the database");
            return existingCourse;
        }

        public static async Task UpdateProgramIdAsync(StudentInfoSystemContext context,int? programID, int existingProgramID)
        {
            if (existingProgramID != programID)
            {
                programID = (programID == null || programID == 0)
                    ? existingProgramID
                    : programID;

                // checking if the ProgramID has changed and validate it
                if (existingProgramID != programID)
                {
                    var existingProgram = await context.Programs
                        .SingleOrDefaultAsync(p => p.ID == programID);

                    if (existingProgram is null)
                        throw new CustomException(400, "Program ID", $"A Program with ID of: '{programID}' not found in the database");
                }
            }
        }

        public static async Task UpdateCourseNameAsync(StudentInfoSystemContext context,Course existingCourse, CourseUpdateDTO courseUpdateDTO)
        {
            if (!string.IsNullOrWhiteSpace(courseUpdateDTO.CourseName))
            {
                //Checking if CourseName is available
                var existingCourseWithSameName = await context.Courses.SingleOrDefaultAsync(c => c.CourseName.Trim().ToLower().Equals(courseUpdateDTO.CourseName.Trim().ToLower()));
                if (existingCourseWithSameName != null && existingCourseWithSameName.ID != existingCourse.ID)
                    throw new CustomException(400, "Course Name", $"A course with name of: '{courseUpdateDTO.CourseName}' already exists in the database");

                //Changing CourseName
                existingCourse.CourseName = courseUpdateDTO.CourseName;
            }
        }

        public static async Task UpdateCourseCodeAsync(StudentInfoSystemContext context,Course existingCourse, CourseUpdateDTO courseUpdateDTO)
        {
            if (!string.IsNullOrWhiteSpace(courseUpdateDTO.CourseCode))
            {
                //Checking if CourseCode is available
                var existingCourseCode = await context.Courses.SingleOrDefaultAsync(c => c.CourseCode.Trim().ToLower().Equals(courseUpdateDTO.CourseCode.Trim().ToLower()));
                if (existingCourseCode != null && existingCourseCode != existingCourse)
                    throw new CustomException(400, "Course Code", $"A course with code of: '{courseUpdateDTO.CourseCode}' already exists in the database");

                //Changing CourseCode
                existingCourse.CourseCode = courseUpdateDTO.CourseCode;
            }
        }
    }
}
