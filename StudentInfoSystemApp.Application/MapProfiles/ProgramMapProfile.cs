using AutoMapper;
using StudentInfoSystemApp.Application.DTOs.ProgramDTOs;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.MapProfiles
{
    public class ProgramMapProfile:Profile
    {
        public ProgramMapProfile()
        {
            CreateMap<Program,ProgramReturnDTO>();
            CreateMap<Student,StudentInProgramReturnDTO>();
            CreateMap<Course, CourseInProgramReturnDTO>();
        }
    }
}
