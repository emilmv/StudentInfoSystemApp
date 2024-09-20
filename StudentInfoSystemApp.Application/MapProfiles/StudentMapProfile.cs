using AutoMapper;
using Microsoft.AspNetCore.Http;
using StudentInfoSystemApp.Application.DTOs.StudentDTOs;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.MapProfiles
{
    public class StudentMapProfile:Profile
    {
        public StudentMapProfile()
        {
            
        }
        public StudentMapProfile(IHttpContextAccessor _httpContextAccessor)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var uriBuilder = new UriBuilder(httpContext.Request.Scheme, httpContext.Request.Host.Host, (int)httpContext.Request.Host.Port);
            var url = uriBuilder.Uri.AbsoluteUri;
            CreateMap<Student, StudentReturnDTO>()
                .ForMember(d => d.DateOfBirth, map => map.MapFrom(s => s.DateOfBirth.ToShortDateString()))
                .ForMember(d => d.EnrollmentDate, map => map.MapFrom(s => s.EnrollmentDate.ToShortDateString()))
                .ForMember(d => d.Photo, map => map.MapFrom(s => url + "images/" + s.Photo));
            CreateMap<Program, ProgramInStudentReturnDTO>();
            CreateMap<Enrollment, EnrollmentInStudentReturnDTO>()
                .ForMember(d=>d.EnrollmentDate,map=>map.MapFrom(s=>s.EnrollmentDate.ToShortDateString()));
        }
    }
}
