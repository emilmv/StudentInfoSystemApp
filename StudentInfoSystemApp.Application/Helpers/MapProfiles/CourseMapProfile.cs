using AutoMapper;
using StudentInfoSystemApp.Application.DTOs.CourseDTOs;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.Helpers.MapProfiles
{
    public class CourseMapProfile : Profile
    {
        public CourseMapProfile()
        {
            //Maps for Return DTO
            CreateMap<Course, CourseReturnDTO>().ForMember(d => d.StudentCount, map => map.MapFrom(s => s.Enrollments.Count()));
            CreateMap<Program, ProgramInCourseReturnDTO>();
            CreateMap<Schedule, ScheduleInCourseReturnDTO>();

            //Maps for Create DTO
            CreateMap<CourseCreateDTO, Course>();
        }
    }
}
