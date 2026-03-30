using AutoMapper;
using Users.Application.DTOs;
using Users.Domain.Aggregates;

namespace Users.Application.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {

        CreateMap<User, UserDTO>()
            .ForCtorParam("UserName", opt => opt.MapFrom(src => src.UserName.Value))
            .ForCtorParam("Email", opt => opt.MapFrom(src => src.Email.Value))
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("Role", opt => opt.MapFrom(src => src.Role));
    }
}