using AutoMapper;
using StudentInfoSystemApp.Application.DTOs.AuthDTOs;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.MapProfiles
{
    public class AuthMapProfile : Profile
    {
        public AuthMapProfile()
        {
            //Register DTO
            CreateMap<RegisterDTO, ApplicationUser>()
                .ForMember(d => d.UserName, map => map.MapFrom(s => s.Username))
                .ForMember(d => d.FullName, map => map.MapFrom(s => s.FirstName + " " + s.LastName))
                .ForMember(d => d.Email, map => map.MapFrom(s => s.Email))
                .ForMember(d => d.PasswordHash, map => map.Ignore())
                .ForMember(d => d.NormalizedEmail, map => map.Ignore())
                .ForMember(d => d.NormalizedUserName, map => map.Ignore())
                .ForMember(d => d.EmailConfirmed, map => map.Ignore());

            //User Return DTO
            CreateMap<ApplicationUser, UserReturnDTO>()
                .ForMember(d => d.Roles, map => map.Ignore());
        }
    }
}
