using AutoMapper;
using StudentInfoSystemApp.Application.DTOs.ProgramDTOs;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.MapProfiles
{
    public class ProgramMapProfile:Profile
    {
        public ProgramMapProfile()
        {
            //Map for ReturnDTO
            CreateMap<Program,ProgramReturnDTO>();
            CreateMap<Student,StudentInProgramReturnDTO>();
            CreateMap<Course, CourseInProgramReturnDTO>();

            //Map for CreateDTO
            CreateMap<ProgramCreateDTO, Program>()
                .ForMember(d => d.ProgramName, map => map.MapFrom(s => s.ProgramName.FirstCharToUpper()))
                .ForMember(d => d.Description, map => map.MapFrom(s => s.Description.FirstCharToUpper()));
        }
    }
}
