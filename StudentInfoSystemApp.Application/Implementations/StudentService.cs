using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.StudentDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;
        public StudentService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
        }

        public async Task<PaginationListDTO<StudentReturnDTO>> GetAllAsync(int page = 1, string searchInput = "")
        {
            //Extracting query to not overload requests
            var query = _studentInfoSystemContext
                .Students
                .Include(s => s.Enrollments)
                .Include(s => s.Program)
                .AsQueryable();

            //Search logic
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
                                        (s.FirstName != null && s.FirstName.ToLower().Contains(searchInput)) ||
                                        (s.LastName != null && s.LastName.ToLower().Contains(searchInput)) ||
                                        ((s.FirstName ?? string.Empty) + " " + (s.LastName ?? string.Empty)).ToLower().Contains(searchInput) ||
                                        (s.Gender!=null&&s.Gender.Trim().ToLower().Contains(searchInput))||
                                        (s.Email != null && s.Email.ToLower().Contains(searchInput)) ||
                                        (s.PhoneNumber != null && s.PhoneNumber.ToLower().Contains(searchInput)) ||
                                        (s.Address != null && s.Address.ToLower().Contains(searchInput)) ||
                                        (s.Status != null && s.Status.ToLower().Contains(searchInput)) ||
                                        (s.Program != null &&
                                         s.Program.ProgramName != null &&
                                         s.Program.ProgramName.ToLower().Contains(searchInput))
                                    );
                }
            }

            var datas = await query
               .Skip((page - 1) * 2)
               .Take(2)
               .ToListAsync();

            var totalCount = await query.CountAsync();

            return new PaginationListDTO<StudentReturnDTO>
            {
                TotalCount = totalCount,
                CurrentPage = page,
                Objects = _mapper.Map<List<StudentReturnDTO>>(datas)
            };
        }
        public async Task<StudentReturnDTO> GetByIdAsync(int? id)
        {
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");
            var student = await _studentInfoSystemContext
                .Students
                .Include(s => s.Enrollments)
                .Include(s => s.Program)
                .FirstOrDefaultAsync(d => d.ID == id);
            if (student is null) throw new CustomException(400, "ID", $"Student with ID of:'{id}'not found in the database");
            return _mapper.Map<StudentReturnDTO>(student);
        }
        public async Task<int> CreateAsync(StudentCreateDTO studentCreateDTO)
        {
            //Validating Program
            var existingProgram = await _studentInfoSystemContext.Programs.SingleOrDefaultAsync(p => p.ID == studentCreateDTO.ProgramID);
            if (existingProgram is null) throw new CustomException(400, "Program ID", $"A program with ID of: '{studentCreateDTO.ProgramID}' not found in the database.");

            //Checking if Student is already registered by the same email
            var existingStudent = await _studentInfoSystemContext.Students.SingleOrDefaultAsync(s => s.Email.Trim().ToLower() == studentCreateDTO.Email.Trim().ToLower());
            if (existingStudent != null) throw new CustomException(400, "Email", $"A student with mail address of: '{studentCreateDTO.Email}' already exists in the database.");

            //Mapping DTO to an object
            Student student = _mapper.Map<Student>(studentCreateDTO);

            //Adding the entity to the database
            await _studentInfoSystemContext.Students.AddAsync(student);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning the ID of the created entity
            return student.ID;
        }
    }
}
