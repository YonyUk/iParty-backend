using MediatR;
using Users.Application.DTOs;

namespace Users.Application.Querys;

public record GetUserByIdQuery(Guid Id):IRequest<UserDTO>;