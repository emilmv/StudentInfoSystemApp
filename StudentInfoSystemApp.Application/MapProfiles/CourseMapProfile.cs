using AutoMapper;
using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.MapProfiles
{
    public class CourseMapProfile : Profile
    {
        public CourseMapProfile()
        {
            CreateMap<Course, CourseReturnDTO>().ForMember(d => d.EnrollmentCount, map => map.MapFrom(s => s.Enrollments.Count()));
            CreateMap<Program, ProgramInCourseReturnDTO>();
            CreateMap<Schedule, ScheduleInCourseReturnDTO>();
        }
    }
}
