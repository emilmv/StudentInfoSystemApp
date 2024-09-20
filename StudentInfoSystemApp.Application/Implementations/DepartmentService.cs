﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.DepartmentDTOs;
using StudentInfoSystemApp.Application.Exceptions;
using StudentInfoSystemApp.Application.Interfaces;
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

        public async Task<List<DepartmentReturnDTO>> GetAllAsync()
        {
            return _mapper.Map<List<DepartmentReturnDTO>>(await _studentInfoSystemContext
                .Departments
                .Include(d => d.Instructors)
                .ToListAsync());
        }

        public async Task<DepartmentReturnDTO> GetByIdAsync(int? id)
        {
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");
            var department = await _studentInfoSystemContext
                .Departments
                .Include (d => d.Instructors)
                .FirstOrDefaultAsync(d => d.ID == id);
            if (department is null) throw new CustomException(400, "ID", $"Department with ID of:'{id}'not found in the database");
            return _mapper.Map<DepartmentReturnDTO>(department);
        }
    }
}