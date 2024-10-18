using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Services.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;
using System.Globalization;

namespace StudentInfoSystemApp.Application.Services.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;
        private readonly IPaginationService<Enrollment> _paginationService;
        public EnrollmentService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper, IPaginationService<Enrollment> paginationService)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
            _paginationService = paginationService;
        }
        public async Task<PaginationListDTO<EnrollmentReturnDTO>> GetAllAsync(int page = 1, string searchInput = "", int pageSize = 3)
        {
            //Extracting query to not overload requests
            var query = CreateEnrollmentQuery(searchInput);

            //Applying pagination
            var pagedEnrollmentDatas = await _paginationService.ApplyPaginationAsync(query, page, pageSize);

            var totalCount = await query.CountAsync();

            return new PaginationListDTO<EnrollmentReturnDTO>
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
                Objects = _mapper.Map<List<EnrollmentReturnDTO>>(pagedEnrollmentDatas)
            };
        }
        public async Task<EnrollmentReturnDTO> GetByIdAsync(int? id)
        {
            //Validating ID
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");

            //Getting Enrollment
            var enrollment = await GetEnrollmentByIdAsync(id.Value);

            //Returning DTO
            return _mapper.Map<EnrollmentReturnDTO>(enrollment);
        }
        public async Task<int> CreateAsync(EnrollmentCreateDTO enrollmentCreateDTO)
        {
            //Checking if Enrollment is duplicate
            await EnsureEnrollmentDoesNotExistAsync(enrollmentCreateDTO.EnrollmentDate, enrollmentCreateDTO.StudentID);

            //Checking if Student exists
            await EnsureStudentExistsAsync(enrollmentCreateDTO.StudentID);

            //Checking if Course exists
            await EnsureCourseExistsAsync(enrollmentCreateDTO.CourseID);

            //Mapping DTO to an object
            Enrollment enrollment = _mapper.Map<Enrollment>(enrollmentCreateDTO);

            //Adding the entity to the database
            await _studentInfoSystemContext.Enrollments.AddAsync(enrollment);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning the ID of the created entity
            return enrollment.ID;
        }
        public async Task<bool> DeleteAsync(int? id)
        {
            //Checking if requested ID is mentioned
            if (id is null) throw new CustomException(400, "ID", "ID cannot be empty");

            //Checking if an Enrollment with requested ID exists in the database
            var existingEnrollment = _studentInfoSystemContext.Enrollments.SingleOrDefault(a => a.ID == id);
            if (existingEnrollment == null) throw new CustomException(400, "ID", $"An enrollment with ID of: '{id}' not found in the database");

            //Deleting the requested attendance
            _studentInfoSystemContext.Enrollments.Remove(existingEnrollment);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning true if delete successful
            return true;
        }
        public async Task<UpdateResponseDTO<EnrollmentReturnDTO>> UpdateAsync(int? id, EnrollmentUpdateDTO enrollmentUpdateDTO)
        {
            //Checking if ID from body is provided
            if (id is null) throw new CustomException(400, "Enrollment ID", "Enrollment ID must not be empty");

            //Finding relevant Enrollment with ID
            var existingEnrollment = await GetEnrollmentByIdAsync(id.Value);

            //Updating StudentID
            await UpdateStudentIdAsync(existingEnrollment, enrollmentUpdateDTO);

            //Updating CourseID
            await UpdateCourseIdAsync(existingEnrollment, enrollmentUpdateDTO);

            //Updating EnrollmentDate
            UpdateEnrollmentDate(existingEnrollment, enrollmentUpdateDTO);

            //Checking for duplication
            await CheckForDuplicationAsync(existingEnrollment.EnrollmentDate, existingEnrollment.StudentID, existingEnrollment.CourseID,existingEnrollment.ID);
            
            //Changing other fields validated from AbstractValidator
            existingEnrollment.Grade = enrollmentUpdateDTO.Grade ?? existingEnrollment.Grade;

            existingEnrollment.Semester = !string.IsNullOrWhiteSpace(enrollmentUpdateDTO.Semester) ? enrollmentUpdateDTO.Semester.FirstCharToUpper() : existingEnrollment.Semester;

            existingEnrollment.StudentID = enrollmentUpdateDTO.StudentID.GetValueOrDefault();

            existingEnrollment.CourseID = enrollmentUpdateDTO.CourseID.GetValueOrDefault();

            // Save changes
            _studentInfoSystemContext.Update(existingEnrollment);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Return response DTO
            return new UpdateResponseDTO<EnrollmentReturnDTO>()
            {
                Response = true,
                UpdateDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<EnrollmentReturnDTO>(existingEnrollment)
            };
        }



        //Private methods
        public IQueryable<Enrollment> CreateEnrollmentQuery(string searchInput)
        {
            //Base query
            var query = _studentInfoSystemContext
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
            return query;
        }
        private async Task<Enrollment> GetEnrollmentByIdAsync(int id)
        {
            var enrollment = await _studentInfoSystemContext
                .Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Include(e => e.Attendances)
                .FirstOrDefaultAsync(d => d.ID == id);

            if (enrollment is null) throw new CustomException(400, "ID", $"Enrollment with ID of:'{id}'not found in the database");

            return enrollment;
        }
        private async Task EnsureEnrollmentDoesNotExistAsync(DateTime enrollmentDate, int studentID)
        {
            var existingEnrollment = await _studentInfoSystemContext.Enrollments.SingleOrDefaultAsync(e => e.EnrollmentDate.Date == enrollmentDate.Date && e.StudentID == studentID);
            if (existingEnrollment != null)
                throw new CustomException(400, "Enrollment", $"An Enrollment with the same date of: '{enrollmentDate.ToShortDateString()}' and Student ID of: '{studentID}' already exists in the database.");
        }
        private async Task EnsureStudentExistsAsync(int studentID)
        {
            var existingStudent = await _studentInfoSystemContext.Students.SingleOrDefaultAsync(s => s.ID == studentID);
            if (existingStudent is null) throw new CustomException(400, "Student ID", $"Student with ID of: '{studentID}' does not exist in the database.");
        }
        private async Task EnsureCourseExistsAsync(int courseID)
        {
            var existingCourse = await _studentInfoSystemContext.Courses.SingleOrDefaultAsync(c => c.ID == courseID);
            if (existingCourse is null)
                throw new CustomException(400, "Course ID", $"Course with ID of: '{courseID}' does not exist in the database.");
        }
        private async Task UpdateStudentIdAsync(Enrollment existingEnrollment, EnrollmentUpdateDTO enrollmentUpdateDTO)
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
                    var existingStudent = await _studentInfoSystemContext.Students.SingleOrDefaultAsync(s => s.ID == enrollmentUpdateDTO.StudentID);
                    if (existingStudent is null) throw new CustomException(400, "Student ID", $"A Student with ID of: '{enrollmentUpdateDTO.StudentID}' not found in the database");
                }
            }
        }
        private async Task UpdateCourseIdAsync(Enrollment existingEnrollment, EnrollmentUpdateDTO enrollmentUpdateDTO)
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
                    var existingCourse = await _studentInfoSystemContext.Courses.SingleOrDefaultAsync(s => s.ID == enrollmentUpdateDTO.CourseID);
                    if (existingCourse is null) throw new CustomException(400, "Course ID", $"A Course with ID of: '{enrollmentUpdateDTO.CourseID}' not found in the database");
                }
            }
        }
        private void UpdateEnrollmentDate(Enrollment existingEnrollment, EnrollmentUpdateDTO enrollmentUpdateDTO)
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
                {
                    throw new CustomException(400, "Enrollment Date", "Invalid date format. Please use dd/MM/yyyy.");
                }
            }
        }
        private async Task CheckForDuplicationAsync(DateTime enrollmentDate, int studentID, int courseID,int enrollmentID)
        {
            var duplicateEnrollment = await _studentInfoSystemContext.Enrollments.SingleOrDefaultAsync(e => e.EnrollmentDate.Date == enrollmentDate.Date && e.StudentID == studentID && e.CourseID == courseID);
            if (duplicateEnrollment != null && duplicateEnrollment.ID != enrollmentID)
                throw new CustomException(400, "Duplicate Enrollment", "An Enrollment with same StudentID, Enrollment Date and CourseID already exists in the database");

        }
    }
}
