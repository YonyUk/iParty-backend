using AutoMapper;
using Users.Application.DTOs;
using Users.Domain.Aggregates;

namespace Users.Application.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {

        CreateMap<User, UserDTO>()
            .ForCtorParam("userName", opt => opt.MapFrom(src => src.UserName.Value))
            .ForCtorParam("email", opt => opt.MapFrom(src => src.Email.Value))
            .ForCtorParam("id", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("role", opt => opt.MapFrom(src => src.Role));
    }
}