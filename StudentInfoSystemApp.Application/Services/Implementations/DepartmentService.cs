using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.CourseDTOs;
using StudentInfoSystemApp.Application.DTOs.DepartmentDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Services.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;
using System.Text.RegularExpressions;

namespace StudentInfoSystemApp.Application.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;
        private readonly IPaginationService<Department> _paginationService;
        public DepartmentService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper, IPaginationService<Department> paginationService)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
            _paginationService = paginationService;
        }

        public async Task<PaginationListDTO<DepartmentReturnDTO>> GetAllAsync(int page = 1, string searchInput = "", int pageSize = 3)
        {
            //Extracting query to not overload requests
            var query = await CreateDepartmentQueryAsync(searchInput);

            //Pagination
            var paginatedDepartmentDatas = await _paginationService.ApplyPaginationAsync(query, page, pageSize);

            var totalCount = await query.CountAsync();

            //Return DTO
            return new PaginationListDTO<DepartmentReturnDTO>
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
                Objects = _mapper.Map<List<DepartmentReturnDTO>>(paginatedDepartmentDatas)
            };
        }
        public async Task<DepartmentReturnDTO> GetByIdAsync(int? id)
        {
            //Validating ID
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty.");

            //Getting department
            var department = await GetDepartmentByIdAsync(id.Value);

            //Return DTO
            return _mapper.Map<DepartmentReturnDTO>(department);
        }
        public async Task<CreateResponseDTO<DepartmentReturnDTO>> CreateAsync(DepartmentCreateDTO departmentCreateDTO)
        {
            //Checking if Department exists in the database
            await EnsureDepartmentDoesNotExistAsync(departmentCreateDTO.DepartmentName);

            //Mapping the DTO to an object
            Department department = _mapper.Map<Department>(departmentCreateDTO);

            //Adding the entity to the database
            await _studentInfoSystemContext.Departments.AddAsync(department);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning the ID of the created entity
            return new CreateResponseDTO<DepartmentReturnDTO>()
            {
                Response = true,
                CreationDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<DepartmentReturnDTO>(department)
            };
        }
        public async Task<bool> DeleteAsync(int? id)
        {
            //Checking if requested ID is mentioned
            if (id is null) throw new CustomException(400, "ID", "ID cannot be empty");

            //Checking if a Department with requested ID exists in the database
            var existingDepartment = _studentInfoSystemContext.Departments.SingleOrDefault(a => a.ID == id);
            if (existingDepartment == null) throw new CustomException(400, "ID", $"A department with ID of: '{id}' not found in the database");

            //Deleting the requested attendance
            _studentInfoSystemContext.Departments.Remove(existingDepartment);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning true if delete successful
            return true;
        }
        public async Task<UpdateResponseDTO<DepartmentReturnDTO>> UpdateAsync(int? id, string departmentName)
        {
            //Checking if ID from body is provided
            if (id is null) throw new CustomException(400, "Department ID", "Department ID must not be empty");

            //Finding relevant Department through ID
            var existingDepartment = await GetDepartmentByIdAsync(id.Value);

            //Checking if DepartmentName is provided
            if (string.IsNullOrWhiteSpace(departmentName) || !Regex.IsMatch(departmentName, "^[a-zA-Z0-9 ]*$")) throw new CustomException(400, "Invalid Department Name", "Department name can only contain letters, numbers, and spaces.");

            //Checking for duplication
            await EnsureDepartmentIsNotDuplicateAsync(existingDepartment.ID, departmentName);

            //Changing name
            existingDepartment.DepartmentName = departmentName.FirstCharToUpper();

            //Save changes
            _studentInfoSystemContext.Update(existingDepartment);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Return DTO
            return new UpdateResponseDTO<DepartmentReturnDTO>()
            {
                Response = true,
                UpdateDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<DepartmentReturnDTO>(existingDepartment)
            };
        }



        //Private methods
        private async Task<IQueryable<Department>> CreateDepartmentQueryAsync(string searchInput)
        {
            var query = _studentInfoSystemContext.Departments
                .Include(d => d.Instructors)
                .AsQueryable();

            //Search logic
            if (!string.IsNullOrWhiteSpace(searchInput.Trim().ToLower()))
                query = query.Where(d => d.DepartmentName.Trim().ToLower().Contains(searchInput.Trim().ToLower()));

            var results = await query.ToListAsync();

            if (!results.Any())
                throw new CustomException(404, "Nothing found", "No records were found matching the search criteria.");
            return query;
        }
        private async Task<Department> GetDepartmentByIdAsync(int id)
        {
            var department = await _studentInfoSystemContext
                .Departments
                .Include(d => d.Instructors)
                .FirstOrDefaultAsync(d => d.ID == id);

            if (department is null) throw new CustomException(400, "ID", $"Department with ID of: '{id}' not found in the database.");

            return department;
        }
        private async Task EnsureDepartmentDoesNotExistAsync(string departmentName)
        {
            var existingDepartment = await _studentInfoSystemContext.Departments.SingleOrDefaultAsync(d => d.DepartmentName.Trim().ToLower() == departmentName.Trim().ToLower());
            if (existingDepartment != null)
                throw new CustomException(400, "DepartmentName", $"A Department with the name of: '{departmentName}' already exists in the database.");
        }
        private async Task EnsureDepartmentIsNotDuplicateAsync(int departmentId, string departmentName)
        {
            var duplicateDepartment = await _studentInfoSystemContext.Departments.FirstOrDefaultAsync(d => d.DepartmentName.Trim().ToLower().Equals(departmentName.Trim().ToLower()));
            if (duplicateDepartment != null && duplicateDepartment.ID != departmentId) throw new CustomException(400, "Department Name", $"A department with name of: '{departmentName}' already exists in the database");
        }
    }
}
