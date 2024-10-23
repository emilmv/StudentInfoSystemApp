using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.DepartmentDTOs;
using StudentInfoSystemApp.Application.DTOs.PaginationDTOs;
using StudentInfoSystemApp.Application.DTOs.ResponseDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Helpers.EntityHelpers;
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
            var query = await DepartmentHelper.CreateDepartmentQueryAsync(_studentInfoSystemContext,searchInput);

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
            var responseDepartment = await DepartmentHelper.GetResponseDepartmentAsync(_studentInfoSystemContext,id.Value);

            //Return DTO
            return _mapper.Map<DepartmentReturnDTO>(responseDepartment);
        }
        public async Task<CreateResponseDTO<DepartmentReturnDTO>> CreateAsync(DepartmentCreateDTO departmentCreateDTO)
        {
            //Checking if Department exists in the database
            await DepartmentHelper.EnsureDepartmentDoesNotExistAsync(_studentInfoSystemContext,departmentCreateDTO.DepartmentName);

            //Mapping the DTO to an object
            Department department = _mapper.Map<Department>(departmentCreateDTO);

            //Adding the entity to the database
            await _studentInfoSystemContext.Departments.AddAsync(department);
            await _studentInfoSystemContext.SaveChangesAsync();

            //Getting responseDTO
            var responseDepartment = await DepartmentHelper.GetResponseDepartmentAsync(_studentInfoSystemContext, department.ID);
            
            //Returning the DTO of the created entity
            return new CreateResponseDTO<DepartmentReturnDTO>()
            {
                Response = true,
                CreationDate = DateTime.Now.ToShortDateString(),
                Objects = _mapper.Map<DepartmentReturnDTO>(responseDepartment)
            };
        }
        public async Task<bool> DeleteAsync(int? id)
        {
            //Checking if requested ID is mentioned
            if (id is null) throw new CustomException(400, "ID", "ID cannot be empty");

            //Checking if a Department with requested ID exists in the database
            var existingDepartment = await DepartmentHelper.FindByIdAsync(_studentInfoSystemContext,id.Value);

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
            var existingDepartment = await DepartmentHelper.GetResponseDepartmentAsync(_studentInfoSystemContext,id.Value);

            //Checking if DepartmentName is provided
            if (string.IsNullOrWhiteSpace(departmentName) || !Regex.IsMatch(departmentName, "^[a-zA-Z0-9 ]*$")) throw new CustomException(400, "Invalid Department Name", "Department name can only contain letters, numbers, and spaces.");

            //Checking for duplication
            await DepartmentHelper.EnsureDepartmentIsNotDuplicateAsync(_studentInfoSystemContext,existingDepartment.ID, departmentName);

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
    }
}
