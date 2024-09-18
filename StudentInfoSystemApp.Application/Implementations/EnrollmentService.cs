using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;
using StudentInfoSystemApp.Application.Interfaces;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;
        public EnrollmentService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
        }
        public async Task<List<EnrollmentReturnDTO>> GetAllAsync()
        {
            return _mapper.Map<List<EnrollmentReturnDTO>>(await _studentInfoSystemContext
                .Enrollments
                .Include(e=>e.Student)
                .Include(e=>e.Course)
                .Include(e=>e.Attendances)
                .ToListAsync());
        }
    }
}
