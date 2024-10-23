using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;
using System.Globalization;

namespace StudentInfoSystemApp.Application.Helpers.EntityHelpers
{
    public static class EnrollmentHelper
    {
        public static async Task<IQueryable<Enrollment>> CreateEnrollmentQueryAsync(StudentInfoSystemContext context, string searchInput)
        {
            //Base query
            var query = context
                .Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Include(e => e.Attendances)
                .AsQueryable();

            // Apply search filter if searchInput is provided
            if (!string.IsNullOrWhiteSpace(searchInput))
            {
                searchInput = searchInput.Trim().ToLower();

                if (DateTime.TryParseExact(searchInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var searchDate))
                {
                    query = query.Where(e => e.EnrollmentDate.Date == searchDate.Date);
                }
                else
                {
                    query = query.Where(e =>
                        e.Grade != null && e.Grade.Trim().ToLower().Contains(searchInput) ||
                        e.Semester != null && e.Semester.Trim().ToLower().Contains(searchInput) ||
                        e.Student != null && (
                            e.Student.FirstName.Trim().ToLower().Contains(searchInput) ||
                            e.Student.LastName.Trim().ToLower().Contains(searchInput) ||
                            e.Student.Email.Trim().ToLower().Contains(searchInput) ||
                            e.Student.Gender.Trim().ToLower() == searchInput ||
                          ((e.Student.FirstName ?? string.Empty) + " " + (e.Student.LastName ?? string.Empty)).ToLower().Contains(searchInput)
                        ) ||
                        e.Course != null && (
                            e.Course.CourseName.Trim().ToLower().Contains(searchInput) ||
                            e.Course.CourseCode.Trim().ToLower().Contains(searchInput)
                        )
                    );
                }
            }
            var results = await query.ToListAsync();

            if (!results.Any())
                throw new CustomException(404, "Nothing found", "No records were found matching the search criteria.");
            return query;
        }

        public static async Task<Enrollment> GetEnrollmentByIdAsync(StudentInfoSystemContext context, int id)
        {
            var enrollment = await context
                .Enrollments
                .FirstOrDefaultAsync(d => d.ID == id);

            if (enrollment is null) throw new CustomException(400, "ID", $"Enrollment with ID of:'{id}'not found in the database");

            return enrollment;
        }

        public static async Task EnsureEnrollmentDoesNotExistAsync(StudentInfoSystemContext context, DateTime enrollmentDate, int studentID)
        {
            var existingEnrollment = await context.Enrollments.SingleOrDefaultAsync(e => e.EnrollmentDate.Date == enrollmentDate.Date && e.StudentID == studentID);
            if (existingEnrollment != null)
                throw new CustomException(400, "Enrollment", $"An Enrollment with the same date of: '{enrollmentDate.ToShortDateString()}' and Student ID of: '{studentID}' already exists in the database.");
        }

        public static async Task EnsureStudentExistsAsync(StudentInfoSystemContext context, int studentID)
        {
            var existingStudent = await context.Students.SingleOrDefaultAsync(s => s.ID == studentID);
            if (existingStudent is null) throw new CustomException(400, "Student ID", $"Student with ID of: '{studentID}' does not exist in the database.");
        }

        public static async Task EnsureCourseExistsAsync(StudentInfoSystemContext context, int courseID)
        {
            var existingCourse = await context.Courses.SingleOrDefaultAsync(c => c.ID == courseID);
            if (existingCourse is null)
                throw new CustomException(400, "Course ID", $"Course with ID of: '{courseID}' does not exist in the database.");
        }

        public static async Task<Enrollment> GetResponseEnrollmentAsync(StudentInfoSystemContext context, int id)
        {
            var enrollment = await context
                .Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Include(e => e.Attendances)
                .FirstOrDefaultAsync(d => d.ID == id);

            if (enrollment is null) throw new CustomException(400, "ID", $"Enrollment with ID of:'{id}'not found in the database");

            return enrollment;
        }

        public static async Task UpdateStudentIdAsync(StudentInfoSystemContext context, Enrollment existingEnrollment, EnrollmentUpdateDTO enrollmentUpdateDTO)
        {
            if (existingEnrollment.StudentID != enrollmentUpdateDTO.StudentID)
            {
                //Checking if null or 0 to keep previous, change if provided
                enrollmentUpdateDTO.StudentID = (enrollmentUpdateDTO.StudentID == null || enrollmentUpdateDTO.StudentID == 0)
                    ? existingEnrollment.StudentID
                    : enrollmentUpdateDTO.StudentID;

                //checking if provided
                if (existingEnrollment.StudentID != enrollmentUpdateDTO.StudentID)
                {
                    //Finding student
                    var existingStudent = await context.Students.SingleOrDefaultAsync(s => s.ID == enrollmentUpdateDTO.StudentID);
                    if (existingStudent is null) throw new CustomException(400, "Student ID", $"A Student with ID of: '{enrollmentUpdateDTO.StudentID}' not found in the database");
                }
            }
        }

        public static async Task UpdateCourseIdAsync(StudentInfoSystemContext context, Enrollment existingEnrollment, EnrollmentUpdateDTO enrollmentUpdateDTO)
        {
            if (existingEnrollment.CourseID != enrollmentUpdateDTO.CourseID)
            {
                //Checking if null or 0 to keep previous, change if provided
                enrollmentUpdateDTO.CourseID = (enrollmentUpdateDTO.CourseID == null || enrollmentUpdateDTO.CourseID == 0)
                    ? existingEnrollment.CourseID
                    : enrollmentUpdateDTO.CourseID;

                //checking if provided
                if (existingEnrollment.CourseID != enrollmentUpdateDTO.CourseID)
                {
                    //Finding Course
                    var existingCourse = await context.Courses.SingleOrDefaultAsync(s => s.ID == enrollmentUpdateDTO.CourseID);
                    if (existingCourse is null) throw new CustomException(400, "Course ID", $"A Course with ID of: '{enrollmentUpdateDTO.CourseID}' not found in the database");
                }
            }
        }

        public static void UpdateEnrollmentDate(Enrollment existingEnrollment, EnrollmentUpdateDTO enrollmentUpdateDTO)
        {
            if (!string.IsNullOrWhiteSpace(enrollmentUpdateDTO.EnrollmentDate))
            {
                if (DateTime.TryParseExact(enrollmentUpdateDTO.EnrollmentDate,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime parsedDate))
                {
                    if (parsedDate.Date > DateTime.Now.Date)
                        throw new CustomException(400, "Enrollment Date", "Enrollment date can not be in the future.");
                    existingEnrollment.EnrollmentDate = parsedDate;
                }
                else
                    throw new CustomException(400, "Enrollment Date", "Invalid date format. Please use dd/MM/yyyy.");
            }
        }

        public static async Task CheckForDuplicationAsync(StudentInfoSystemContext context, DateTime enrollmentDate, int studentID, int courseID, int enrollmentID)
        {
            var duplicateEnrollment = await context.Enrollments.SingleOrDefaultAsync(e => e.EnrollmentDate.Date == enrollmentDate.Date && e.StudentID == studentID && e.CourseID == courseID);
            if (duplicateEnrollment != null && duplicateEnrollment.ID != enrollmentID)
                throw new CustomException(400, "Duplicate Enrollment", "An Enrollment with same StudentID, Enrollment Date and CourseID already exists in the database");

        }
    }
}

