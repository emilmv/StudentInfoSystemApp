using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.StudentDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;
using System.Globalization;

namespace StudentInfoSystemApp.Application.Helpers.EntityHelpers
{
    public static class StudentHelper
    {
        public static async Task<IQueryable<Student>> CreateStudentQueryAsync(StudentInfoSystemContext context, string searchInput)
        {
            var query = context
                .Students
                .Include(s => s.Enrollments)
                .ThenInclude(se => se.Course)
                .Include(s => s.Program)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchInput))
            {
                if (DateTime.TryParseExact(searchInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var searchDate))
                {
                    query = query.Where(e =>
                        e.EnrollmentDate.Date == searchDate.Date ||
                        e.DateOfBirth.Date == searchDate.Date);
                }
                else
                {
                    searchInput = searchInput.Trim().ToLower();
                    query = query.Where(s =>
                        (s.FirstName ?? "").ToLower().Contains(searchInput) ||
                        (s.LastName ?? "").ToLower().Contains(searchInput) ||
                        ((s.FirstName ?? "") + " " + (s.LastName ?? "")).ToLower().Contains(searchInput) ||
                        (s.Gender ?? "").Trim().ToLower().Contains(searchInput) ||
                        (s.Email ?? "").ToLower().Contains(searchInput) ||
                        (s.PhoneNumber ?? "").ToLower().Contains(searchInput) ||
                        (s.Address ?? "").ToLower().Contains(searchInput) ||
                        (s.Status ?? "").ToLower().Contains(searchInput) ||
                        (s.Program != null && (s.Program.ProgramName ?? "").ToLower().Contains(searchInput)));
                }
            }
            var results = await query.ToListAsync();

            if (!results.Any())
                throw new CustomException(404, "Nothing found", "No records were found matching the search criteria.");

            return query;
        }

        public static async Task<Student> GetResponseStudentAsync(StudentInfoSystemContext context, int id)
        {
            var student =await context
                .Students
                .Include(s => s.Enrollments)
                .ThenInclude(se => se.Course)
                .Include(s => s.Program)
                .FirstOrDefaultAsync(s=>s.ID == id);

            if (student == null) throw new CustomException(400, "Student ID", $"A Student with ID of: '{id}' not found in the database");

            return student;
        }

        public static async Task<Student> GetExistingStudentAsync(StudentInfoSystemContext context, int id)
        {
            var existingStudent =await context
                .Students
                .FirstOrDefaultAsync (s=>s.ID == id);

            if (existingStudent == null) throw new CustomException(400, "Student ID", $"A Student with ID of: '{id}' not found in the database");

            return existingStudent;
        }

        public static async Task ValidateProgramUpdateAsync(StudentInfoSystemContext context, Student existingStudent, StudentUpdateDTO studentUpdateDTO)
        {
            if (existingStudent.ProgramID != studentUpdateDTO.ProgramID)
            {
                studentUpdateDTO.ProgramID = (studentUpdateDTO.ProgramID == null || studentUpdateDTO.ProgramID == 0)
                    ? existingStudent.ProgramID
                    : studentUpdateDTO.ProgramID;

                if (existingStudent.ProgramID != studentUpdateDTO.ProgramID)
                {
                    var existingProgram = await context.Programs.SingleOrDefaultAsync(s => s.ID == studentUpdateDTO.ProgramID);
                    if (existingProgram is null) throw new CustomException(400, "Program ID", $"A Program with ID of: '{studentUpdateDTO.ProgramID}' not found in the database");
                }
            }
        }

        public static void UpdateDateOfBirth(Student existingStudent, StudentUpdateDTO studentUpdateDTO)
        {
            if (!string.IsNullOrWhiteSpace(studentUpdateDTO.DateOfBirth))
            {
                if (DateTime.TryParseExact(studentUpdateDTO.DateOfBirth, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    if (parsedDate.Year > DateTime.Now.AddYears(-16).Year)
                        throw new CustomException(400, "Date of birth", "Student must be at least 16 years old to register");
                    existingStudent.DateOfBirth = parsedDate;
                }
                else
                    throw new CustomException(400, "Date of birth", "Invalid date format. Please use dd/MM/yyyy.");
            }
        }

        public static void UpdateEnrollmentDate(Student existingStudent, StudentUpdateDTO studentUpdateDTO)
        {
            if (!string.IsNullOrWhiteSpace(studentUpdateDTO.EnrollmentDate))
            {
                if (DateTime.TryParseExact(studentUpdateDTO.EnrollmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    if (parsedDate.Date > DateTime.Now.Date)
                        throw new CustomException(400, "Enrollment date", "Enrollment date must be in the past");
                    existingStudent.EnrollmentDate = parsedDate;
                }
                else
                    throw new CustomException(400, "Enrollment date", "Invalid date format. Please use dd/MM/yyyy.");
            }
        }

        public static async Task ValidateEmailUpdateAsync(StudentInfoSystemContext context, Student existingStudent, StudentUpdateDTO studentUpdateDTO)
        {
            if (!string.IsNullOrEmpty(studentUpdateDTO.Email))
            {
                var duplicateEmail = await context.Students.SingleOrDefaultAsync(e => e.Email == studentUpdateDTO.Email);
                if (duplicateEmail != null && duplicateEmail.ID != existingStudent.ID)
                    throw new CustomException(400, "Email", $"A Student with email address of: '{studentUpdateDTO.Email}' already exists in the database.");
                existingStudent.Email = studentUpdateDTO.Email;
            }
        }
        public static async Task CheckAvailablePhoneNumberAsync(StudentInfoSystemContext context, string phoneNumber)
        {
            var existingStudent = await context.Students.FirstOrDefaultAsync(s => s.PhoneNumber.Trim()== phoneNumber.Trim());
            if (existingStudent != null)
                throw new CustomException(400, "PhoneNumber", $"A student with phone number of: '{phoneNumber}' already exists in the database.");
        }
        public static async Task ValidatePhoneNumberUpdateAsync(StudentInfoSystemContext context, Student existingStudent, StudentUpdateDTO studentUpdateDTO)
        {
            if (!string.IsNullOrEmpty(studentUpdateDTO.PhoneNumber))
            {
                var duplicatePhoneNumber = await context.Students.SingleOrDefaultAsync(s => s.PhoneNumber.Trim() == studentUpdateDTO.PhoneNumber.Trim());
                if (duplicatePhoneNumber != null && duplicatePhoneNumber.ID != existingStudent.ID)
                    throw new CustomException(400, "Phone Number", $"A Student with phone number of: '{studentUpdateDTO.PhoneNumber}' already exists in the database.");
                existingStudent.PhoneNumber = studentUpdateDTO.PhoneNumber;
            }
        }

        public static async Task ValidateProgramAsync(StudentInfoSystemContext context, int programId)
        {
            var existingProgram = await context.Programs.SingleOrDefaultAsync(p => p.ID == programId);
            if (existingProgram is null)
                throw new CustomException(400, "Program ID", $"A program with ID of: '{programId}' not found in the database.");
        }

        public static async Task ValidateAvailableEmailAsync(StudentInfoSystemContext context, string email)
        {
            var existingStudent = await context.Students.FirstOrDefaultAsync(s => s.Email.Trim().ToLower() == email.Trim().ToLower());
            if (existingStudent != null)
                throw new CustomException(400, "Email", $"A student with mail address of: '{email}' already exists in the database.");
        }
    }
}
