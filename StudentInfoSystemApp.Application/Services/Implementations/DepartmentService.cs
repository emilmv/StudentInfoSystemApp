using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.DepartmentDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Services.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;
        public DepartmentService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
        }

        public async Task<PaginationListDTO<DepartmentReturnDTO>> GetAllAsync(int page = 1, string searchInput = "")
        {
            //Extracting query to not overload requests
            var query = _studentInfoSystemContext.Departments
                .Include(d => d.Instructors)
                .AsQueryable();

            //Search logic
            if (!string.IsNullOrWhiteSpace(searchInput.Trim().ToLower()))
                query = query.Where(d => d.DepartmentName.Trim().ToLower().Contains(searchInput.Trim().ToLower()));

            var datas = await query
                .Skip((page - 1) * 2)
                .Take(2)
                .ToListAsync();

            var totalCount = await query.CountAsync();

            return new PaginationListDTO<DepartmentReturnDTO>
            {
                TotalCount = totalCount,
                CurrentPage = page,
                Objects = _mapper.Map<List<DepartmentReturnDTO>>(datas)
            };
        }

        public async Task<DepartmentReturnDTO> GetByIdAsync(int? id)
        {
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty.");
            var department = await _studentInfoSystemContext
                .Departments
                .Include(d => d.Instructors)
                .FirstOrDefaultAsync(d => d.ID == id);
            if (department is null) throw new CustomException(400, "ID", $"Department with ID of:'{id}'not found in the database.");
            return _mapper.Map<DepartmentReturnDTO>(department);
        }

        public async Task<int> CreateAsync(DepartmentCreateDTO departmentCreateDTO)
        {
            //Checking if Department exists in the database
            var existingDepartment = await _studentInfoSystemContext.Departments.SingleOrDefaultAsync(d => d.DepartmentName.Trim().ToLower() == departmentCreateDTO.DepartmentName.Trim().ToLower());
            if (existingDepartment != null)
                throw new CustomException(400, "DepartmentName", $"A Department with the name of: '{departmentCreateDTO.DepartmentName}' already exists in the database.");

            //Mapping the DTO to an object
            Department department = _mapper.Map<Department>(departmentCreateDTO);

            //Adding the entity to the database
            await _studentInfoSystemContext.Departments.AddAsync(department);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Returning the ID of the created entity
            return department.ID;
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
    }
}
