using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;
using StudentInfoSystemApp.Application.DTOs.StudentDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Extensions;
using StudentInfoSystemApp.Application.Services.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;
using System.Globalization;

namespace StudentInfoSystemApp.Application.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;
        private readonly IPaginationService<Student> _paginationService;
        public StudentService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper, IPaginationService<Student> paginationService)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
            _paginationService = paginationService;
        }

        public async Task<PaginationListDTO<StudentReturnDTO>> GetAllAsync(int page = 1, string searchInput = "", int pageSize = 3)
        {
            //Creating the query based on searchInput
            var query = await CreateStudentQueryAsync(searchInput);

            //Applying Pagination logic
            var paginatedStudents = await _paginationService.ApplyPaginationAsync(query, page, pageSize);

            //Total count of students based on searchInput
            var totalCount = await query.CountAsync();

            //Returning PaginationDTO
            return new PaginationListDTO<StudentReturnDTO>
            {
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize=pageSize,
                Objects = _mapper.Map<List<StudentReturnDTO>>(paginatedStudents)
            };
        }
        public async Task<StudentReturnDTO> GetByIdAsync(int? id)
        {
            //Validating if ID from body is provided
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");

            //Finding student with relevant ID
            var student = await GetExistingStudentAsync(id.Value);

            //Returning DTO
            return _mapper.Map<StudentReturnDTO>(student);
        }
        public async Task<CreateResponseDTO<StudentReturnDTO>> CreateAsync(StudentCreateDTO studentCreateDTO)
        {
            //Validating Program
            await ValidateProgramAsync(studentCreateDTO.ProgramID);

            //Checking if Student is already registered by the same email
            await ValidateAvailableEmailAsync(studentCreateDTO.Email);

            //Mapping DTO to an object
            Student student = _mapper.Map<Student>(studentCreateDTO);

            //Adding the entity to the database
            await _studentInfoSystemContext.Students.AddAsync(student);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning ResponseDTO of the created entity
            return new CreateResponseDTO<StudentReturnDTO>()
            {
                Response = true,
                CreationDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<StudentReturnDTO>(student)
            };
        }
        public async Task<bool> DeleteAsync(int? id)
        {
            //Checking if requested ID is mentioned
            if (id is null) throw new CustomException(400, "ID", "ID cannot be empty");

            //Checking if a Student with requested ID exists in the database
            var existingStudent = await GetExistingStudentAsync(id.Value);

            //Deleting the requested attendance
            _studentInfoSystemContext.Students.Remove(existingStudent);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning true if delete successful
            return true;
        }
        public async Task<UpdateResponseDTO<StudentReturnDTO>> UpdateAsync(int? id, StudentUpdateDTO studentUpdateDTO)
        {
            // Check if ID from body is provided
            if (id is null) throw new CustomException(400, "Student ID", "Student ID must not be empty");

            // Finding the Student
            var existingStudent = await GetExistingStudentAsync(id.Value);

            //Updating fields that require logic
            await ValidateProgramUpdateAsync(existingStudent, studentUpdateDTO);
            UpdateDateOfBirth(existingStudent, studentUpdateDTO);
            UpdateEnrollmentDate(existingStudent, studentUpdateDTO);
            await ValidateEmailUpdateAsync(existingStudent, studentUpdateDTO);
            await ValidatePhoneNumberUpdateAsync(existingStudent, studentUpdateDTO);

            //Updating the other fields that don't require logic
            existingStudent.FirstName = string.IsNullOrEmpty(studentUpdateDTO.FirstName)
                ? existingStudent.FirstName
                : studentUpdateDTO.FirstName.FirstCharToUpper();

            existingStudent.LastName = string.IsNullOrEmpty(studentUpdateDTO.LastName)
                ? existingStudent.LastName
                : studentUpdateDTO.LastName.FirstCharToUpper();

            existingStudent.Gender = string.IsNullOrEmpty(studentUpdateDTO.Gender)
                ? existingStudent.Gender
                : studentUpdateDTO.Gender.FirstCharToUpper();

            existingStudent.Address = string.IsNullOrEmpty(studentUpdateDTO.Address)
                ? existingStudent.Address
                : studentUpdateDTO.Address.FirstCharToUpper();

            existingStudent.Status = string.IsNullOrEmpty(studentUpdateDTO.Status)
                ? existingStudent.Status
                : studentUpdateDTO.Status.FirstCharToUpper();

            if (studentUpdateDTO.Photo != null)
                existingStudent.Photo = studentUpdateDTO.Photo.Save(existingStudent.FirstName.FirstCharToUpper(), existingStudent.LastName.FirstCharToUpper(), Directory.GetCurrentDirectory(), "images");

            // Save changes
            _studentInfoSystemContext.Update(existingStudent);
            await _studentInfoSystemContext.SaveChangesAsync();

            // Return response DTO
            return new UpdateResponseDTO<StudentReturnDTO>()
            {
                Response = true,
                UpdateDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<StudentReturnDTO>(existingStudent)
            };
        }



        //Private methods to refactor main methods
        private async Task<IQueryable<Student>> CreateStudentQueryAsync(string searchInput)
        {
            var query = _studentInfoSystemContext
                .Students
                .Include(s => s.Enrollments)
                .ThenInclude(se=>se.Course)
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
        private async Task<Student> GetExistingStudentAsync(int id)
        {
            var query = _studentInfoSystemContext
                .Students
                .Include(s => s.Enrollments)
                .ThenInclude(se => se.Course)
                .Include(s => s.Program)
                .AsQueryable();

            var existingStudent = await query.FirstOrDefaultAsync(s => s.ID == id);
            if (existingStudent == null) throw new CustomException(400, "Student ID", $"A Student with ID of: '{id}' not found in the database");

            return existingStudent;
        }
        private async Task ValidateProgramUpdateAsync(Student existingStudent, StudentUpdateDTO studentUpdateDTO)
        {
            if (existingStudent.ProgramID != studentUpdateDTO.ProgramID)
            {
                studentUpdateDTO.ProgramID = (studentUpdateDTO.ProgramID == null || studentUpdateDTO.ProgramID == 0)
                    ? existingStudent.ProgramID
                    : studentUpdateDTO.ProgramID;

                if (existingStudent.ProgramID != studentUpdateDTO.ProgramID)
                {
                    var existingProgram = await _studentInfoSystemContext.Programs.SingleOrDefaultAsync(s => s.ID == studentUpdateDTO.ProgramID);
                    if (existingProgram is null) throw new CustomException(400, "Program ID", $"A Program with ID of: '{studentUpdateDTO.ProgramID}' not found in the database");
                }
            }
        }
        private void UpdateDateOfBirth(Student existingStudent, StudentUpdateDTO studentUpdateDTO)
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
        private void UpdateEnrollmentDate(Student existingStudent, StudentUpdateDTO studentUpdateDTO)
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
        private async Task ValidateEmailUpdateAsync(Student existingStudent, StudentUpdateDTO studentUpdateDTO)
        {
            if (!string.IsNullOrEmpty(studentUpdateDTO.Email))
            {
                var duplicateEmail = await _studentInfoSystemContext.Students.SingleOrDefaultAsync(e => e.Email == studentUpdateDTO.Email);
                if (duplicateEmail != null && duplicateEmail.ID != existingStudent.ID)
                    throw new CustomException(400, "Email", $"A Student with email address of: '{studentUpdateDTO.Email}' already exists in the database.");
                existingStudent.Email = studentUpdateDTO.Email;
            }
        }
        private async Task ValidatePhoneNumberUpdateAsync(Student existingStudent, StudentUpdateDTO studentUpdateDTO)
        {
            if (!string.IsNullOrEmpty(studentUpdateDTO.PhoneNumber))
            {
                var duplicatePhoneNumber = await _studentInfoSystemContext.Students.SingleOrDefaultAsync(s => s.PhoneNumber.Trim() == studentUpdateDTO.PhoneNumber.Trim());
                if (duplicatePhoneNumber != null && duplicatePhoneNumber.ID != existingStudent.ID)
                    throw new CustomException(400, "Phone Number", $"A Student with phone number of: '{studentUpdateDTO.PhoneNumber}' already exists in the database.");
                existingStudent.PhoneNumber = studentUpdateDTO.PhoneNumber;
            }
        }
        private async Task ValidateProgramAsync(int programId)
        {
            var existingProgram = await _studentInfoSystemContext.Programs.SingleOrDefaultAsync(p => p.ID == programId);
            if (existingProgram is null)
                throw new CustomException(400, "Program ID", $"A program with ID of: '{programId}' not found in the database.");
        }
        private async Task ValidateAvailableEmailAsync(string email)
        {
            var existingStudent = await _studentInfoSystemContext.Students.SingleOrDefaultAsync(s => s.Email.Trim().ToLower() == email.Trim().ToLower());
            if (existingStudent != null)
                throw new CustomException(400, "Email", $"A student with mail address of: '{email}' already exists in the database.");
        }
        
    }
}
