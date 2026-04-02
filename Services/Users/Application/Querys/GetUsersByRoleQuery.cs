using MediatR;
using Users.Application.DTOs;
using Users.Domain;

namespace Users.Application.Querys;

public record GetUsersByRoleQuery(UserRole Role):IRequest<IEnumerable<UserDTO>>;