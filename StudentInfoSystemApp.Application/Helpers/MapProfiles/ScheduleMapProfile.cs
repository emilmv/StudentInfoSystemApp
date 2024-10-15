using AutoMapper;
using StudentInfoSystemApp.Application.DTOs.ScheduleDTOs;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.Helpers.MapProfiles
{
    public class ScheduleMapProfile : Profile
    {
        public ScheduleMapProfile()
        {
            //Map for ReturnDTO
            CreateMap<Schedule, ScheduleReturnDTO>();
            CreateMap<Course, CourseInScheduleReturnDTO>();
            CreateMap<Instructor, InstructorInScheduleReturnDTO>();

            //Map for CreateDTO
            CreateMap<ScheduleCreateDTO, Schedule>()
                .ForMember(d => d.Semester, map => map.MapFrom(s => s.Semester.FirstCharToUpper()))
                .ForMember(d => d.Classroom, map => map.MapFrom(s => s.Classroom.FirstCharToUpper()));
        }
    }
}
