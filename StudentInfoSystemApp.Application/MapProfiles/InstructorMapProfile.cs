using AutoMapper;
using Microsoft.AspNetCore.Http;
using StudentInfoSystemApp.Application.DTOs.InstructorDTOs;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.MapProfiles
{
    public class InstructorMapProfile:Profile
    {
        public InstructorMapProfile()
        {
            
        }
        public InstructorMapProfile(IHttpContextAccessor _httpContextAccessor)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var uriBuilder = new UriBuilder(httpContext.Request.Scheme, httpContext.Request.Host.Host, (int)httpContext.Request.Host.Port);
            var url = uriBuilder.Uri.AbsoluteUri;
            CreateMap<Instructor, InstructorReturnDTO>()
                .ForMember(d => d.HireDate, map => map.MapFrom(s => s.HireDate.ToShortDateString()))
                .ForMember(d => d.Photo, map => map.MapFrom(s => url + "images/" + s.Photo));
            CreateMap<Department, DepartmentInInstructorReturnDTO>()
                .ForMember(d => d.DepartmentName, map => map.MapFrom(s => s.DepartmentName.Trim()));
            CreateMap<Schedule, ScheduleInInstructorReturnDTO>()
                .ForMember(d=>d.CourseName,map=>map.MapFrom(s=>s.Course.CourseName));
        }
    }
}
