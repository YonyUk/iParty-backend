using MediatR;
using Users.Application.DTOs;

namespace Users.Application.Commands;

public record GetUserByIdCommand(Guid Id):IRequest<UserDTO>;