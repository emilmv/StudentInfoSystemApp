using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentInfoSystemApp.Application.DTOs.ProgramDTOs;
using StudentInfoSystemApp.Application.DTOs.ScheduleDTOs;
using StudentInfoSystemApp.Application.Interfaces;
using StudentInfoSystemApp.DataAccess.Data;

namespace StudentInfoSystemApp.Application.Implementations
{
    public class ScheduleService:IScheduleService
    {
        private readonly StudentInfoSystemContext _studentInfoSystemContext;
        private readonly IMapper _mapper;
        public ScheduleService(StudentInfoSystemContext studentInfoSystemContext, IMapper mapper)
        {
            _studentInfoSystemContext = studentInfoSystemContext;
            _mapper = mapper;
        }

        public async Task<List<ScheduleReturnDTO>> GetAllAsync()
        {
            return _mapper.Map<List<ScheduleReturnDTO>>(await _studentInfoSystemContext
                .Schedules
                .Include(s=>s.Course)
                .Include(s=>s.Instructor)
                .ToListAsync());
        }
    }
}
