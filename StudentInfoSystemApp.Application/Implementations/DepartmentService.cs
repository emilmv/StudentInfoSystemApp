using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.CourseDTOs;
using StudentInfoSystemApp.Application.DTOs.DepartmentDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Implementations
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

        public async Task<DepartmentListDTO> GetAllAsync(int page = 1, string searchInput = "")
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

            DepartmentListDTO departmentListDTO = new();
            departmentListDTO.TotalCount = totalCount;
            departmentListDTO.CurrentPage = page;
            departmentListDTO.Departments = _mapper.Map<List<DepartmentReturnDTO>>(datas);

            return departmentListDTO;
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
    }
}
