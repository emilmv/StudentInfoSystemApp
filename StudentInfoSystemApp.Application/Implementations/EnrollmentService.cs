using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs;
using StudentInfoSystemApp.Application.Exceptions;
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

        public async Task<EnrollmentReturnDTO> GetByIdAsync(int? id)
        {
            if (id is null) throw new CustomException(400, "ID", "ID must not be empty");
            var enrollment = await _studentInfoSystemContext
                .Enrollments
                .Include(e=>e.Student)
                .Include(e=>e.Course)
                .Include (e=>e.Attendances)
                .FirstOrDefaultAsync(d => d.ID == id);
            if (enrollment is null) throw new CustomException(400, "ID", $"Enrollment with ID of:'{id}'not found in the database");
            return _mapper.Map<EnrollmentReturnDTO>(enrollment);
        }
    }
}
