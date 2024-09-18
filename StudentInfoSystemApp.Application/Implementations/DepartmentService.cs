using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;
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
    }
}
