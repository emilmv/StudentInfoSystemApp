using AutoMapper;
using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.MapProfiles
{
    public class DepartmentMapProfile:Profile
    {
        public DepartmentMapProfile()
        {
            CreateMap<Department, DepartmentReturnDTO>();
            CreateMap<Instructor, InstructorsInDepartmentReturnDTO>()
                .ForMember(d=>d.HireDate,map=>map.MapFrom(s=>s.HireDate.ToShortDateString()));
        }
    }
}
