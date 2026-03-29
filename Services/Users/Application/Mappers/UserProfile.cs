using AutoMapper;
using Users.Application.DTOs;
using Users.Domain.Aggregates;

namespace Users.Application.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDTO>()
            .MaxDepth(5)
            .ForMember(dto => dto.UserName, opt => opt.MapFrom(user => user.UserName.Value))
            .ForMember(dto => dto.Email, opt => opt.MapFrom(user => user.Email.Value));
    }
}