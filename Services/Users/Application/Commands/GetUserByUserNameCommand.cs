using MediatR;
using Users.Application.DTOs;
using Users.Domain.ValueObjects;

namespace Users.Application.Commands;

public record GetUserByUserNameCommand(string UserName):IRequest<UserDTO>;