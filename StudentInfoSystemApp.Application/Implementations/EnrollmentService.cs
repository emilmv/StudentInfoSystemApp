using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.DepartmentDTOs;
using StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Implementations
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
    }
}
