using AutoMapper;
using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.Helpers.MapProfiles
{
    public class AttendanceMapProfile : Profile
    {
        public AttendanceMapProfile()
        {
            //Return DTO
            CreateMap<Attendance, AttendanceReturnDTO>().ForMember(d => d.AttendanceDate, map => map.MapFrom(s => s.AttendanceDate.ToShortDateString()));
            CreateMap<Enrollment, EnrollmentInAttendanceReturnDTO>().ForMember(d => d.CourseRegistrationDate, map => map.MapFrom(s => s.EnrollmentDate.ToShortDateString()))
                .ForMember(d => d.StudentFullName, map => map.MapFrom(s => s.Student.FirstName + " " + s.Student.LastName))
                .ForMember(d=>d.CourseName,map=>map.MapFrom(s=>s.Course.CourseName));

            //Create DTO
            CreateMap<AttendanceCreateDTO, Attendance>()
                .ForMember(d => d.Status, map => map.MapFrom(s => s.Status.FirstCharToUpper()));

            //Update DTO
            CreateMap<AttendanceUpdateDTO, Attendance>()
                .ForMember(d => d.Status, map => map.MapFrom(s => s.Status.FirstCharToUpper()));
        }
    }
}
