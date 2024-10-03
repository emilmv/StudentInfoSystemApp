using AutoMapper;
using Microsoft.AspNetCore.Http;
using StudentInfoSystemApp.Application.DTOs.InstructorDTOs;
using StudentInfoSystemApp.Application.DTOs.StudentDTOs;
using StudentInfoSystemApp.Application.Extensions;
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
            //Map for ReturnDTO
            var httpContext = _httpContextAccessor.HttpContext;
            var uriBuilder = new UriBuilder(httpContext.Request.Scheme, httpContext.Request.Host.Host, (int)httpContext.Request.Host.Port);
            var url = uriBuilder.Uri.AbsoluteUri;
            CreateMap<Student, StudentReturnDTO>()
                .ForMember(d => d.DateOfBirth, map => map.MapFrom(s => s.DateOfBirth.ToShortDateString()))
                .ForMember(d => d.EnrollmentDate, map => map.MapFrom(s => s.EnrollmentDate.ToShortDateString()))
                .ForMember(d => d.Photo, map => map.MapFrom(s => url + "images/" + s.Photo));
            CreateMap<Program, ProgramInStudentReturnDTO>();
            CreateMap<Enrollment, EnrollmentInStudentReturnDTO>()
                .ForMember(d=>d.CourseRegistrationDate,map=>map.MapFrom(s=>s.EnrollmentDate.ToShortDateString()));

            //Map for CreateDTO
            CreateMap<StudentCreateDTO, Student>()
                .ForMember(d => d.Photo, map => map.MapFrom(s => s.Photo.Save(s.FirstName.ToLower(), s.LastName.ToLower(), Directory.GetCurrentDirectory(), "images")))
                .ForMember(d => d.FirstName, map => map.MapFrom(s => s.FirstName.FirstCharToUpper()))
                .ForMember(d => d.LastName, map => map.MapFrom(s => s.LastName.FirstCharToUpper()))
                .ForMember(d => d.Gender, map => map.MapFrom(s => s.Gender.FirstCharToUpper()))
                .ForMember(d => d.Address, map => map.MapFrom(s => s.Address.FirstCharToUpper()))
                .ForMember(d => d.Status, map => map.MapFrom(s => s.Status.FirstCharToUpper()));
        }
    }
}
