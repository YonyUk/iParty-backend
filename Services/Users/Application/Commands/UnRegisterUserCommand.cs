using MediatR;

namespace Users.Application.Commands;

public record UnRegisterUserCommand(Guid Id):IRequest<Unit>;