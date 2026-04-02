using MediatR;
using Users.Application.DTOs;

namespace Users.Application.Querys;

public record GetUsersQuery:IRequest<IEnumerable<UserDTO>>;