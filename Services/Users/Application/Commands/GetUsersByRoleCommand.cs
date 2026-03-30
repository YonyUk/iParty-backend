using MediatR;
using Users.Application.DTOs;
using Users.Domain;

namespace Users.Application.Commands;

public record GetUsersByRoleCommand(UserRole Role):IRequest<IEnumerable<UserDTO>>;