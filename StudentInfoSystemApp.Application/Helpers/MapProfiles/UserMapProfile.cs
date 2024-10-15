using AutoMapper;
using StudentInfoSystemApp.Application.DTOs.AuthDTOs;
using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.Helpers.MapProfiles
{
    public class UserMapProfile : Profile
    {
        public UserMapProfile()
        {
            CreateMap<ApplicationUser, UserReturnDTO>();
        }
    }
}
