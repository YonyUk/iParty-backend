using MediatR;
using Users.Application.DTOs;

namespace Users.Application.Querys;

public record GetUserByUserNameQuery(string UserName):IRequest<UserDTO>;