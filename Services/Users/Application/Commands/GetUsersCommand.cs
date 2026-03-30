using MediatR;
using Users.Application.DTOs;

namespace Users.Application.Commands;

public record GetUsersCommand:IRequest<IEnumerable<UserDTO>>;