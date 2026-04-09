using MediatR;
using Users.Application.DTOs;

namespace Users.Application.Commands;

public record ChangePasswordCommand(Guid Id,string password):IRequest<Unit>;