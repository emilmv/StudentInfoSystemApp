using AutoMapper;
using StudentInfoSystemApp.Application.DTOs.ScheduleDTOs;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.MapProfiles
{
    public class ScheduleMapProfile:Profile
    {
        public ScheduleMapProfile()
        {
            CreateMap<Schedule,ScheduleReturnDTO>();
            CreateMap<Course,CourseInScheduleReturnDTO>();
            CreateMap<Instructor, InstructorInScheduleReturnDTO>();
        }
    }
}
