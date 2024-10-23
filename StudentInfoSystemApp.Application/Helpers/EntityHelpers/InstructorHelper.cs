using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.InstructorDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;
using System.Globalization;

namespace StudentInfoSystemApp.Application.Helpers.EntityHelpers
{
    public static class InstructorHelper
    {
        public static async Task<IQueryable<Instructor>> CreateInstructorQueryAsync(StudentInfoSystemContext context, string searchInput)
        {
            // Base query for instructors
            var query = context
                .Instructors
                .Include(i => i.Department)
                .Include(i => i.Schedules)
                .ThenInclude(ii => ii.Course)
                .AsQueryable();

            // Apply search filter if searchInput is provided
            if (!string.IsNullOrWhiteSpace(searchInput))
            {
                searchInput = searchInput.Trim().ToLower();
                query = query.Where(i =>
                    (i.FirstName != null && i.FirstName.Trim().ToLower().Contains(searchInput)) ||
                    (i.LastName != null && i.LastName.Trim().ToLower().Contains(searchInput)) ||
                    (i.Email != null && i.Email.Trim().ToLower().Contains(searchInput)) ||
                    (i.PhoneNumber != null && i.PhoneNumber.Trim().ToLower().Contains(searchInput)) ||
                    (i.Department != null && i.Department.DepartmentName != null && i.Department.DepartmentName.Trim().ToLower().Contains(searchInput)) ||
                    (i.Schedules.Any(s =>
                        (s.ClassTime != null && s.ClassTime.Trim().ToLower().Contains(searchInput)) ||
                        (s.Semester != null && s.Semester.Trim().ToLower().Contains(searchInput)) ||
                        (s.Classroom != null && s.Classroom.Trim().ToLower().Contains(searchInput)) ||
                        (s.Course.CourseName != null && s.Course.CourseName.Trim().ToLower().Contains(searchInput))
                    )));
            }
            var results = await query.ToListAsync();

            if (!results.Any())
                throw new CustomException(404, "Nothing found", "No records were found matching the search criteria.");
            return query;
        }

        public static async Task<Instructor> GetResponseInstructorAsync(StudentInfoSystemContext context, int id)
        {
            var instructor = await context
                .Instructors
                .Include(i => i.Department)
                .Include(i => i.Schedules)
                .ThenInclude(ii => ii.Course)
                .FirstOrDefaultAsync(d => d.ID == id);

            if (instructor == null)
                throw new CustomException(400, "ID", $"Instructor with ID of: '{id}' not found in the database.");

            return instructor;
        }

        public static async Task EnsureInstructorDoesNotExistAsync(StudentInfoSystemContext context, string email)
        {
            var existingInstructor = await context
                .Instructors
                .SingleOrDefaultAsync(i => i.Email == email);

            if (existingInstructor != null)
                throw new CustomException(400, "Email", $"An instructor with email address of: '{email}' already exists in the database.");
        }

        public static async Task EnsureDepartmentExistsAsync(StudentInfoSystemContext context, int departmentId)
        {
            var existingDepartment = await context.Departments.SingleOrDefaultAsync(d => d.ID == departmentId);

            if (existingDepartment == null)
                throw new CustomException(400, "Department ID", $"Department with ID of: '{departmentId}' not found in the database.");
        }

        public static async Task UpdateDepartmentIdAsync(StudentInfoSystemContext context, Instructor existingInstructor, InstructorUpdateDTO instructorUpdateDTO)
        {
            if (existingInstructor.DepartmentID != instructorUpdateDTO.DepartmentID)
            {
                instructorUpdateDTO.DepartmentID = instructorUpdateDTO.DepartmentID ?? existingInstructor.DepartmentID;

                if (instructorUpdateDTO.DepartmentID != existingInstructor.DepartmentID)
                {
                    var existingDepartment = await context.Departments
                        .SingleOrDefaultAsync(d => d.ID == instructorUpdateDTO.DepartmentID);

                    if (existingDepartment is null)
                        throw new CustomException(400, "Department ID", $"A Department with ID of: '{instructorUpdateDTO.DepartmentID}' not found in the database");
                }
            }
        }

        public static void UpdateHireDate(Instructor existingInstructor, InstructorUpdateDTO instructorUpdateDTO)
        {
            if (!string.IsNullOrWhiteSpace(instructorUpdateDTO.HireDate))
            {
                if (DateTime.TryParseExact(instructorUpdateDTO.HireDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    if (parsedDate.Date > DateTime.Now.Date)
                    {
                        throw new CustomException(400, "Hire Date", "Hire date cannot be in the future.");
                    }
                    existingInstructor.HireDate = parsedDate;
                }
                else
                    throw new CustomException(400, "Hire Date", "Invalid date format. Please use dd/MM/yyyy.");
            }
        }

        public static async Task UpdateEmailAsync(StudentInfoSystemContext context, Instructor existingInstructor, InstructorUpdateDTO instructorUpdateDTO)
        {
            if (!string.IsNullOrEmpty(instructorUpdateDTO.Email))
            {
                var duplicateEmail = await context.Instructors
                    .SingleOrDefaultAsync(e => e.Email == instructorUpdateDTO.Email);

                if (duplicateEmail != null && duplicateEmail != existingInstructor)
                    throw new CustomException(400, "Email", $"An Instructor with email address of: '{instructorUpdateDTO.Email}' already exists in the database.");

                existingInstructor.Email = instructorUpdateDTO.Email;
            }
        }

        public static async Task UpdatePhoneNumberAsync(StudentInfoSystemContext context, Instructor existingInstructor, InstructorUpdateDTO instructorUpdateDTO)
        {
            if (!string.IsNullOrEmpty(instructorUpdateDTO.PhoneNumber))
            {
                var duplicatePhoneNumber = await context.Instructors
                    .SingleOrDefaultAsync(e => e.PhoneNumber.Trim() == instructorUpdateDTO.PhoneNumber.Trim());

                if (duplicatePhoneNumber != null && duplicatePhoneNumber != existingInstructor)
                    throw new CustomException(400, "Phone Number", $"An Instructor with phone number of: '{instructorUpdateDTO.PhoneNumber}' already exists in the database.");

                existingInstructor.PhoneNumber = instructorUpdateDTO.PhoneNumber;
            }
        }
    
        public static async Task<Instructor> GetInstructorByIdAsync(StudentInfoSystemContext context, int id)
        {
            var instructor=await context.Instructors.FirstOrDefaultAsync(i=>i.ID==id);

            if (instructor is null) throw new CustomException(400, "ID", $"An Instructor with ID of:'{id}'not found in the database");

            return instructor;
        }
    }
}
