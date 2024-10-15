using AutoMapper;
using StudentInfoSystemApp.Application.DTOs.DepartmentDTOs;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.Helpers.MapProfiles
{
    public class DepartmentMapProfile : Profile
    {
        public DepartmentMapProfile()
        {
            //Maps for Return DTO
            CreateMap<Department, DepartmentReturnDTO>()
                .ForMember(d => d.DepartmentName, map => map.MapFrom(s => s.DepartmentName.Trim()));
            CreateMap<Instructor, InstructorsInDepartmentReturnDTO>()
                .ForMember(d => d.HireDate, map => map.MapFrom(s => s.HireDate.ToShortDateString()));
            //Maps for Create DTO
            CreateMap<DepartmentCreateDTO, Department>();
        }
    }
}
