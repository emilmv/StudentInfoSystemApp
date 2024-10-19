using AutoMapper;
using Microsoft.AspNetCore.Http;
using StudentInfoSystemApp.Application.DTOs.EnrollmentDTOs;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.Helpers.MapProfiles
{
    public class EnrollmentMapProfile : Profile
    {
        public EnrollmentMapProfile()
        {

        }
        public EnrollmentMapProfile(IHttpContextAccessor _httpContextAccessor)
        {
            //Map for Return DTO
            var httpContext = _httpContextAccessor.HttpContext;
            var uriBuilder = new UriBuilder(httpContext.Request.Scheme, httpContext.Request.Host.Host, (int)httpContext.Request.Host.Port);
            var url = uriBuilder.Uri.AbsoluteUri;
            CreateMap<Enrollment, EnrollmentReturnDTO>()
                .ForMember(d => d.EnrollmentDate, map => map.MapFrom(s => s.EnrollmentDate.ToShortDateString()));
            CreateMap<Student, StudentInEnrollmentReturnDTO>()
                .ForMember(d => d.DateOfBirth, map => map.MapFrom(s => s.DateOfBirth.ToShortDateString()))
                .ForMember(d => d.Photo, map => map.MapFrom(s => url + "images/" + s.Photo));
            CreateMap<Course, CourseInEnrollmentReturnDTO>();

            //Map for Create DTO
            CreateMap<EnrollmentCreateDTO, Enrollment>()
                .ForMember(d => d.Grade, map => map.MapFrom(s => s.Grade.FirstCharToUpper()))
                .ForMember(d => d.Semester, map => map.MapFrom(s => s.Semester.FirstCharToUpper()));
        }
    }
}
