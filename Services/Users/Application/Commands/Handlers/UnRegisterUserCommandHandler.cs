using Common.Domain;
using MediatR;
using Users.Domain;

namespace Users.Application.Commands.Handlers;

public class UnRegisterUserCommandHandler : IRequestHandler<UnRegisterUserCommand,Unit>
{
    private readonly IUserRepository userRepository;
    private readonly IUnitOfWork unitOfWork;
    public UnRegisterUserCommandHandler(IUserRepository userRepository,IUnitOfWork unitOfWork)
    {
        this.userRepository = userRepository;
        this.unitOfWork = unitOfWork;
    }
    public async Task<Unit> Handle(UnRegisterUserCommand request, CancellationToken cancellationToken)
    {
        await userRepository.Delete(request.Id,cancellationToken);
        await unitOfWork.Commit(cancellationToken);
        return Unit.Value;
    }
}