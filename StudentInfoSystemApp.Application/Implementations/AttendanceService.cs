using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;
using StudentInfoSystemApp.Application.Interfaces;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;
        public AttendanceService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
        }

        public async Task<List<AttendanceReturnDTO>> GetAllAsync()
        {
            return _mapper.Map<List<AttendanceReturnDTO>>(await _studentInfoSystemContext
                .Attendances
                .Include(a => a.Enrollment)
                .ThenInclude(e => e.Student)
                .ToListAsync());
        }
    }
}
