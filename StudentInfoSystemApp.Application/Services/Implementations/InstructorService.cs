using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs;
using StudentInfoSystemApp.Application.DTOs.InstructorDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Extensions;
using StudentInfoSystemApp.Application.Services.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;
using System.Globalization;

namespace StudentInfoSystemApp.Application.Services.Implementations
{
    public class InstructorService : IInstructorService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;

        public InstructorService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
        }

        public async Task<PaginationListDTO<InstructorReturnDTO>> GetAllAsync(int page = 1, string searchInput = "")
        {
            //Extracting query to not overload requests
            var query = _studentInfoSystemContext
                .Instructors
                .Include(i => i.Department)
                .Include(i => i.Schedules)
                .ThenInclude(ii => ii.Course)
                .AsQueryable();

            //Search logic
            if (!string.IsNullOrWhiteSpace(searchInput))
            {
                if (DateTime.TryParseExact(searchInput.Trim().ToLower(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var searchDate))
                {
                    query = query.Where(i => i.HireDate.Date == searchDate.Date);
                }
                else
                {
                    searchInput = searchInput.Trim().ToLower();
                    query = query.Where(i =>
                        i.FirstName != null && i.FirstName.Trim().ToLower().Contains(searchInput) ||
                        i.LastName != null && i.LastName.Trim().ToLower().Contains(searchInput) ||
                        ((i.FirstName ?? "") + " " + (i.LastName ?? "")).ToLower().Contains(searchInput) ||
                        i.Email != null && i.Email.Trim().ToLower().Contains(searchInput) ||
                        i.PhoneNumber != null && i.PhoneNumber.Trim().ToLower().Contains(searchInput) ||
                        i.Department != null &&
                            i.Department.DepartmentName != null &&
                             i.Department.DepartmentName.Trim().ToLower().Contains(searchInput) ||
                        i.Schedules.Any(s => s.ClassTime != null && s.ClassTime.Trim().ToLower().Contains(searchInput)) ||
                         i.Schedules.Any(s => s.Semester != null && s.Semester.Trim().ToLower().Contains(searchInput)) ||
                         i.Schedules.Any(s => s.Classroom != null && s.Classroom.Trim().ToLower().Contains(searchInput)) ||
                         i.Schedules.Any(s => s.Course.CourseName != null && s.Course.CourseName.Trim().ToLower().Contains(searchInput))
                    );
                }
            }

            var datas = await query
               .Skip((page - 1) * 2)
               .Take(2)
               .ToListAsync();

            var totalCount = await query.CountAsync();

            return new PaginationListDTO<InstructorReturnDTO>
            {
                TotalCount = totalCount,
                CurrentPage = page,
                Objects = _mapper.Map<List<InstructorReturnDTO>>(datas)
            };
        }

        public async Task<InstructorReturnDTO> GetByIdAsync(int? id)
        {
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");
            var instructor = await _studentInfoSystemContext
                .Instructors
                .Include(i => i.Department)
                .Include(i => i.Schedules)
                .ThenInclude(ii => ii.Course)
                .FirstOrDefaultAsync(d => d.ID == id);
            if (instructor is null) throw new CustomException(400, "ID", $"Instructor with ID of:'{id}'not found in the database");
            return _mapper.Map<InstructorReturnDTO>(instructor);
        }

        public async Task<int> CreateAsync(InstructorCreateDTO instructorCreateDTO)
        {
            //Extracting query into a variable not to use 2 requests in 1 method
            var query = _studentInfoSystemContext.Instructors;

            //Checking if Instructor with same mail address exists in the database
            var existingInstructor = await query.SingleOrDefaultAsync(i => i.Email == instructorCreateDTO.Email);
            if (existingInstructor != null) throw new CustomException(400, "Email", $"An instructor with maill address of: '{instructorCreateDTO.Email}' already exists in the database.");

            //Checking if Department exists in the database
            var existingDepartment = await _studentInfoSystemContext.Departments.SingleOrDefaultAsync(d => d.ID == instructorCreateDTO.DepartmentID);
            if (existingDepartment is null) throw new CustomException(400, "Department ID", $"Department with ID of: '{instructorCreateDTO.DepartmentID}' not found in the database.");

            //Mapping DTO to an object
            Instructor instructor = _mapper.Map<Instructor>(instructorCreateDTO);

            //Adding the entity to the database
            await _studentInfoSystemContext.Instructors.AddAsync(instructor);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning the ID of the created entity
            return instructor.ID;
        }
        public async Task<bool> DeleteAsync(int? id)
        {
            //Checking if requested ID is mentioned
            if (id is null) throw new CustomException(400, "ID", "ID cannot be empty");

            //Checking if an Instructor with requested ID exists in the database
            var existingInstructor = _studentInfoSystemContext.Instructors.SingleOrDefault(a => a.ID == id);
            if (existingInstructor == null) throw new CustomException(400, "ID", $"An instructor with ID of: '{id}' not found in the database");

            //Deleting the requested attendance
            _studentInfoSystemContext.Instructors.Remove(existingInstructor);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning true if delete successful
            return true;
        }

        public async Task<UpdateResponseDTO<InstructorReturnDTO>> UpdateAsync(int? id, InstructorUpdateDTO instructorUpdateDTO)
        {
            //extracting query
            var query = _studentInfoSystemContext
                .Instructors
                .Include(i => i.Department)
                .Include(i => i.Schedules)
                .ThenInclude(ii => ii.Course)
                .AsQueryable();

            //Checking if ID from body is provided
            if (id is null) throw new CustomException(400, "Instructor ID", "Instructor ID must not be empty");

            //Finding relevant Instructor with ID
            var existingInstructor = await query.FirstOrDefaultAsync(i => i.ID == id);
            if (existingInstructor == null) throw new CustomException(400, "Instructor ID", $"An Instructor with ID of: '{id}' not found in the database");

            //Checking if DepartmentID is changed
            if (existingInstructor.DepartmentID != instructorUpdateDTO.DepartmentID)
            {
                //Checking if null or 0 to keep previous, change if provided
                instructorUpdateDTO.DepartmentID = (instructorUpdateDTO.DepartmentID == null || instructorUpdateDTO.DepartmentID == 0)
                    ? existingInstructor.DepartmentID
                    : instructorUpdateDTO.DepartmentID;

                //checking if provided
                if (existingInstructor.DepartmentID != instructorUpdateDTO.DepartmentID)
                {
                    //Finding Department
                    var existingDepartment = await _studentInfoSystemContext.Departments.SingleOrDefaultAsync(s => s.ID == instructorUpdateDTO.DepartmentID);
                    if (existingDepartment is null) throw new CustomException(400, "Department ID", $"A Department with ID of: '{instructorUpdateDTO.DepartmentID}' not found in the database");
                }
            }
            //Checking if Date is changed
            if (!string.IsNullOrWhiteSpace(instructorUpdateDTO.HireDate))
            {
                if (DateTime.TryParseExact(instructorUpdateDTO.HireDate,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime parsedDate))
                {
                    if (parsedDate.Date > DateTime.Now.Date)
                        throw new CustomException(400, "Hire Date", "Hire date can not be in the future.");
                    existingInstructor.HireDate = parsedDate;
                }
                else
                    throw new CustomException(400, "Hire Date", "Invalid date format. Please use dd/MM/yyyy.");
            }
            //Checking if Email is changed
            if (!string.IsNullOrEmpty(instructorUpdateDTO.Email))
            {
                var duplicateEmail = await query.SingleOrDefaultAsync(e => e.Email == instructorUpdateDTO.Email);
                if (duplicateEmail != null && duplicateEmail != existingInstructor) throw new CustomException(400, "Email", $"An Instructor with email address of: '{instructorUpdateDTO.Email}' already exists in the database.");
                existingInstructor.Email = instructorUpdateDTO.Email;
            }
            //Checking if Phone number is changed
            if (!string.IsNullOrEmpty(instructorUpdateDTO.PhoneNumber))
            {
                var duplicatePhoneNumber = await query.SingleOrDefaultAsync(e => e.PhoneNumber.Trim() == instructorUpdateDTO.PhoneNumber.Trim());
                if (duplicatePhoneNumber != null && duplicatePhoneNumber != existingInstructor) throw new CustomException(400, "Phone Number", $"An Instructor with phone number of: '{instructorUpdateDTO.PhoneNumber}' already exists in the database.");
                existingInstructor.PhoneNumber = instructorUpdateDTO.PhoneNumber;
            }

            //Updating
            existingInstructor.DepartmentID = instructorUpdateDTO.DepartmentID.GetValueOrDefault();
            existingInstructor.FirstName = string.IsNullOrEmpty(instructorUpdateDTO.FirstName)
                ? existingInstructor.FirstName
                : instructorUpdateDTO.FirstName;
            existingInstructor.LastName = string.IsNullOrEmpty(instructorUpdateDTO.LastName)
                ? existingInstructor.LastName
                : instructorUpdateDTO.LastName;
            existingInstructor.Photo = instructorUpdateDTO.Photo == null ? existingInstructor.Photo : instructorUpdateDTO.Photo.Save(existingInstructor.FirstName, existingInstructor.LastName, Directory.GetCurrentDirectory(), "images");

            // Save changes
            _studentInfoSystemContext.Update(existingInstructor);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Return response DTO
            return new UpdateResponseDTO<InstructorReturnDTO>()
            {
                Response = true,
                UpdateDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<InstructorReturnDTO>(existingInstructor)
            };
        }
    }
}
