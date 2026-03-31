using MediatR;
using Users.Application.DTOs;

namespace Users.Application.Commands;

public record LoginUserCommand(LoginUserDTO data):IRequest<LoginResponseDTO>;