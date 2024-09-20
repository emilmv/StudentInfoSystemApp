using AutoMapper;
using StudentInfoSystemApp.Application.DTOs.DepartmentDTOs;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.MapProfiles
{
    public class DepartmentMapProfile:Profile
    {
        public DepartmentMapProfile()
        {
            CreateMap<Department, DepartmentReturnDTO>()
                .ForMember(d => d.DepartmentName, map => map.MapFrom(s => s.DepartmentName.Trim()));
            CreateMap<Instructor, InstructorsInDepartmentReturnDTO>()
                .ForMember(d=>d.HireDate,map=>map.MapFrom(s=>s.HireDate.ToShortDateString()));
        }
    }
}
