using AutoMapper;
using StudentInfoSystemApp.Application.DTOs.AttendanceDTOs;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.MapProfiles
{
    public class AttendanceMapProfile:Profile
    {
        public AttendanceMapProfile()
        {
            CreateMap<Attendance, AttendanceReturnDTO>().ForMember(d=>d.AttendanceDate,map=>map.MapFrom(s=>s.AttendanceDate.ToShortDateString()));
            CreateMap<Enrollment, EnrollmentInAttendanceReturnDTO>().ForMember(d => d.EnrollmentDate, map => map.MapFrom(s => s.EnrollmentDate.ToShortDateString()))
                .ForMember(d => d.StudentFullName,map=>map.MapFrom(s=>s.Student.FirstName+" "+s.Student.LastName));
        }
    }
}
