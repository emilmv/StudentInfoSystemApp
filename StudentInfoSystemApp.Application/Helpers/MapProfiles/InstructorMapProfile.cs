using AutoMapper;
using Microsoft.AspNetCore.Http;
using StudentInfoSystemApp.Application.DTOs.InstructorDTOs;
using StudentInfoSystemApp.Application.Extensions;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.Helpers.MapProfiles
{
    public class InstructorMapProfile : Profile
    {
        public InstructorMapProfile()
        {

        }
        public InstructorMapProfile(IHttpContextAccessor _httpContextAccessor)
        {
            //Map for ReturnDTO
            var httpContext = _httpContextAccessor.HttpContext;
            var uriBuilder = new UriBuilder(httpContext.Request.Scheme, httpContext.Request.Host.Host, (int)httpContext.Request.Host.Port);
            var url = uriBuilder.Uri.AbsoluteUri;
            CreateMap<Instructor, InstructorReturnDTO>()
                .ForMember(d => d.HireDate, map => map.MapFrom(s => s.HireDate.ToShortDateString()))
                .ForMember(d => d.Photo, map => map.MapFrom(s => url + "images/" + s.Photo));
            CreateMap<Department, DepartmentInInstructorReturnDTO>()
                .ForMember(d => d.DepartmentName, map => map.MapFrom(s => s.DepartmentName.Trim()));
            CreateMap<Schedule, ScheduleInInstructorReturnDTO>()
                .ForMember(d => d.CourseName, map => map.MapFrom(s => s.Course.CourseName));

            //Map for CreateDTO
            CreateMap<InstructorCreateDTO, Instructor>()
                .ForMember(d => d.Photo, map => map.MapFrom(s => s.Photo.Save(s.FirstName.ToLower(), s.LastName.ToLower(), Directory.GetCurrentDirectory(), "images")))
                .ForMember(d => d.FirstName, map => map.MapFrom(s => s.FirstName.FirstCharToUpper()))
                .ForMember(d => d.LastName, map => map.MapFrom(s => s.LastName.FirstCharToUpper()));
        }
    }
}
