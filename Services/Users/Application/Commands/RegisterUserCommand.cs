using MediatR;
using Users.Application.DTOs;

namespace Users.Application.Commands;

public record RegisterUserCommand(RegisterUserDTO data):IRequest<UserCreatedResponseDTO>;