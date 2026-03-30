using MediatR;
using Users.Application.DTOs;

namespace Users.Application.Commands;

public record GetUserByEmailCommand(string Email):IRequest<UserDTO>;