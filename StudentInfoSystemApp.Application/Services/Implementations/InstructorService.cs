using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        private readonly IPaginationService<Instructor> _paginationService;

        public InstructorService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper, IPaginationService<Instructor> paginationService)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
            _paginationService = paginationService;
        }

        public async Task<PaginationListDTO<InstructorReturnDTO>> GetAllAsync(int page = 1, string searchInput = "", int pageSize = 3)
        {
            //Extracting query to not overload requests
            var query =await CreateInstructorQueryAsync(searchInput);

            //Applying pagination
            var pagedInstructorDatas = await _paginationService.ApplyPaginationAsync(query, page, pageSize);

            var totalCount = await query.CountAsync();

            return new PaginationListDTO<InstructorReturnDTO>
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
                Objects = _mapper.Map<List<InstructorReturnDTO>>(pagedInstructorDatas)
            };
        }
        public async Task<InstructorReturnDTO> GetByIdAsync(int? id)
        {
            //Validating ID
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");

            //Getting the Instructor
            var instructor = await GetInstructorByIdAsync(id.Value);

            //Returning DTO
            return _mapper.Map<InstructorReturnDTO>(instructor);
        }
        public async Task<CreateResponseDTO<InstructorReturnDTO>> CreateAsync(InstructorCreateDTO instructorCreateDTO)
        {
            //Checking if Instructor with same mail address exists in the database
            await EnsureInstructorDoesNotExistAsync(instructorCreateDTO.Email);

            //Checking if Department exists in the database
            await EnsureDepartmentExistsAsync(instructorCreateDTO.DepartmentID);

            //Mapping DTO to an object
            Instructor instructor = _mapper.Map<Instructor>(instructorCreateDTO);

            //Adding the entity to the database
            await _studentInfoSystemContext.Instructors.AddAsync(instructor);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning the ID of the created entity
            return new CreateResponseDTO<InstructorReturnDTO>
            {
                Response = true,
                CreationDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<InstructorReturnDTO>(instructor)
            };
        }
        public async Task<UpdateResponseDTO<InstructorReturnDTO>> UpdateAsync(int? id, InstructorUpdateDTO instructorUpdateDTO)
        {
            //Checking if ID from body is provided
            if (id is null) throw new CustomException(400, "Instructor ID", "Instructor ID must not be empty");

            //Finding relevant Instructor with ID
            var existingInstructor = await GetInstructorByIdAsync(id.Value);

            //Updating DepartmentID
            await UpdateDepartmentIdAsync(existingInstructor, instructorUpdateDTO);

            //Updating HireDate
            UpdateHireDate(existingInstructor, instructorUpdateDTO);

            //Updating Email
            await UpdateEmailAsync(existingInstructor, instructorUpdateDTO);

            //Updating Phone number
            await UpdatePhoneNumberAsync(existingInstructor, instructorUpdateDTO);

            //Updating DepartmentID if provided
            existingInstructor.DepartmentID = instructorUpdateDTO.DepartmentID.GetValueOrDefault();

            //Updating First name
            existingInstructor.FirstName = string.IsNullOrEmpty(instructorUpdateDTO.FirstName)
                ? existingInstructor.FirstName
                : instructorUpdateDTO.FirstName;

            //Updating Last name
            existingInstructor.LastName = string.IsNullOrEmpty(instructorUpdateDTO.LastName)
                ? existingInstructor.LastName
                : instructorUpdateDTO.LastName;

            //Updating photo
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
        public async Task<bool> DeleteAsync(int? id)
        {
            //Checking if requested ID is mentioned
            if (id is null) throw new CustomException(400, "ID", "ID cannot be empty");

            //Checking if an Instructor with requested ID exists in the database
            var existingInstructor = _studentInfoSystemContext.Instructors.SingleOrDefault(a => a.ID == id);
            if (existingInstructor == null) throw new CustomException(400, "ID", $"An instructor with ID of: '{id}' not found in the database");

            //Deleting the requested Instructor
            _studentInfoSystemContext.Instructors.Remove(existingInstructor);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning true if delete successful
            return true;
        }
        



        //Private methods
        private async Task<IQueryable<Instructor>> CreateInstructorQueryAsync(string searchInput)
        {
            // Base query for instructors
            var query = _studentInfoSystemContext
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
        private async Task<Instructor> GetInstructorByIdAsync(int id)
        {
            var instructor = await _studentInfoSystemContext
                .Instructors
                .Include(i => i.Department)
                .Include(i => i.Schedules)
                .ThenInclude(ii => ii.Course)
                .FirstOrDefaultAsync(d => d.ID == id);

            if (instructor == null)
                throw new CustomException(400, "ID", $"Instructor with ID of: '{id}' not found in the database");

            return instructor;
        }
        private async Task EnsureInstructorDoesNotExistAsync(string email)
        {
            var existingInstructor = await _studentInfoSystemContext
                .Instructors
                .SingleOrDefaultAsync(i => i.Email == email);

            if (existingInstructor != null)
                throw new CustomException(400, "Email", $"An instructor with email address of: '{email}' already exists in the database.");
        }
        private async Task EnsureDepartmentExistsAsync(int departmentId)
        {
            var existingDepartment = await _studentInfoSystemContext.Departments.SingleOrDefaultAsync(d => d.ID == departmentId);

            if (existingDepartment == null)
                throw new CustomException(400, "Department ID", $"Department with ID of: '{departmentId}' not found in the database.");
        }
        private async Task UpdateDepartmentIdAsync(Instructor existingInstructor, InstructorUpdateDTO instructorUpdateDTO)
        {
            if (existingInstructor.DepartmentID != instructorUpdateDTO.DepartmentID)
            {
                instructorUpdateDTO.DepartmentID = instructorUpdateDTO.DepartmentID ?? existingInstructor.DepartmentID;

                if (instructorUpdateDTO.DepartmentID != existingInstructor.DepartmentID)
                {
                    var existingDepartment = await _studentInfoSystemContext.Departments
                        .SingleOrDefaultAsync(d => d.ID == instructorUpdateDTO.DepartmentID);

                    if (existingDepartment is null)
                        throw new CustomException(400, "Department ID", $"A Department with ID of: '{instructorUpdateDTO.DepartmentID}' not found in the database");
                }
            }
        }
        private void UpdateHireDate(Instructor existingInstructor, InstructorUpdateDTO instructorUpdateDTO)
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
        private async Task UpdateEmailAsync(Instructor existingInstructor, InstructorUpdateDTO instructorUpdateDTO)
        {
            if (!string.IsNullOrEmpty(instructorUpdateDTO.Email))
            {
                var duplicateEmail = await _studentInfoSystemContext.Instructors
                    .SingleOrDefaultAsync(e => e.Email == instructorUpdateDTO.Email);

                if (duplicateEmail != null && duplicateEmail != existingInstructor)
                    throw new CustomException(400, "Email", $"An Instructor with email address of: '{instructorUpdateDTO.Email}' already exists in the database.");

                existingInstructor.Email = instructorUpdateDTO.Email;
            }
        }
        private async Task UpdatePhoneNumberAsync(Instructor existingInstructor, InstructorUpdateDTO instructorUpdateDTO)
        {
            if (!string.IsNullOrEmpty(instructorUpdateDTO.PhoneNumber))
            {
                var duplicatePhoneNumber = await _studentInfoSystemContext.Instructors
                    .SingleOrDefaultAsync(e => e.PhoneNumber.Trim() == instructorUpdateDTO.PhoneNumber.Trim());

                if (duplicatePhoneNumber != null && duplicatePhoneNumber != existingInstructor)
                    throw new CustomException(400, "Phone Number", $"An Instructor with phone number of: '{instructorUpdateDTO.PhoneNumber}' already exists in the database.");

                existingInstructor.PhoneNumber = instructorUpdateDTO.PhoneNumber;
            }
        }
    }
}
