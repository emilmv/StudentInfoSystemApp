using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;
using StudentInfoSystemApp.Application.DTOs.CourseDTOs;
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
        public EnrollmentService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
        }
        public async Task<PaginationListDTO<EnrollmentReturnDTO>> GetAllAsync(int page = 1, string searchInput = "")
        {
            //Extracting query to not overload requests
            var query = _studentInfoSystemContext
                .Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Include(e => e.Attendances)
                .AsQueryable();

            //Search logic
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

            var datas = await query
               .Skip((page - 1) * 2)
               .Take(2)
               .ToListAsync();

            var totalCount = await query.CountAsync();

            return new PaginationListDTO<EnrollmentReturnDTO>
            {
                TotalCount = totalCount,
                CurrentPage = page,
                Objects = _mapper.Map<List<EnrollmentReturnDTO>>(datas)
            };
        }

        public async Task<EnrollmentReturnDTO> GetByIdAsync(int? id)
        {
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");
            var enrollment = await _studentInfoSystemContext
                .Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Include(e => e.Attendances)
                .FirstOrDefaultAsync(d => d.ID == id);
            if (enrollment is null) throw new CustomException(400, "ID", $"Enrollment with ID of:'{id}'not found in the database");
            return _mapper.Map<EnrollmentReturnDTO>(enrollment);
        }
        public async Task<int> CreateAsync(EnrollmentCreateDTO enrollmentCreateDTO)
        {
            //Extracting query into a variable not to use 2 requests in 1 method
            var query = _studentInfoSystemContext.Enrollments;

            //Checking if Enrollment exists in the database
            var existingEnrollment = await query.SingleOrDefaultAsync(e => e.EnrollmentDate.Date == enrollmentCreateDTO.EnrollmentDate.Date && e.StudentID == enrollmentCreateDTO.StudentID);
            if (existingEnrollment != null)
                throw new CustomException(400, "Enrollment", $"An Enrollment with the same date of: '{enrollmentCreateDTO.EnrollmentDate.ToShortDateString()}' and Student ID of: '{enrollmentCreateDTO.StudentID}' already exists in the database.");

            //Checking if Student does not exist in the database
            var existingStudent = await _studentInfoSystemContext.Students.SingleOrDefaultAsync(s => s.ID == enrollmentCreateDTO.StudentID);
            if (existingStudent is null) throw new CustomException(400, "Student ID", $"Student with ID of: '{enrollmentCreateDTO.StudentID}' does not exist in the database.");

            //Checking if Course does not exist in the database
            var existingCourse = await _studentInfoSystemContext.Courses.SingleOrDefaultAsync(c => c.ID == enrollmentCreateDTO.CourseID);
            if (existingCourse is null)
                throw new CustomException(400, "Course ID", $"Course with ID of: '{enrollmentCreateDTO.CourseID}' does not exist in the database.");

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
            //extracting query
            var query = _studentInfoSystemContext
                .Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Include(e => e.Attendances)
                .AsQueryable();

            //Checking if ID from body is provided
            if (id is null) throw new CustomException(400, "Enrollment ID", "Enrollment ID must not be empty");

            //Finding relevant Enrollment with ID
            var existingEnrollment = await query.FirstOrDefaultAsync(e => e.ID == id);
            if (existingEnrollment is null) throw new CustomException(400, "Enrollment ID", $"An Enrollment with ID of: '{id}' not found in the database");

            //Checking if StudentID is changed to validate
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
            //Same thing for Course
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
            //Checking if Date is changed
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
            //Checking for duplication
            var duplicateEnrollment = await query.SingleOrDefaultAsync(e => e.EnrollmentDate.Date == existingEnrollment.EnrollmentDate.Date && e.StudentID == existingEnrollment.StudentID && e.CourseID == existingEnrollment.CourseID);
            if (duplicateEnrollment != null && duplicateEnrollment != existingEnrollment)
                throw new CustomException(400, "Duplicate Enrollment", "An Enrollment with same StudentID, Enrollment Date and CourseID already exists in the database");

            //Changing other fields validated from AbstractValidator
            if (!string.IsNullOrWhiteSpace(enrollmentUpdateDTO.Grade))
                existingEnrollment.Grade = enrollmentUpdateDTO.Grade;

            if (!string.IsNullOrWhiteSpace(enrollmentUpdateDTO.Semester))
                existingEnrollment.Semester = enrollmentUpdateDTO.Semester.FirstCharToUpper();

            existingEnrollment.StudentID= enrollmentUpdateDTO.StudentID.GetValueOrDefault();

            existingEnrollment.CourseID=enrollmentUpdateDTO.CourseID.GetValueOrDefault();

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
    }
}
