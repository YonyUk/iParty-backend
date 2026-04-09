using Common.Domain;
using MediatR;
using Users.Application.Services;
using Users.Domain;
using Users.Domain.Rules;
using Users.Domain.ValueObjects;

namespace Users.Application.Commands.Handlers;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand,Unit>
{
    private readonly IUserDomainRulesConfigProvider userDomainRulesConfigProvider;
    private readonly IUserRepository userRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IPasswordHasher passwordHasher;
    public ChangePasswordCommandHandler(
        IUserDomainRulesConfigProvider userDomainRulesConfigProvider,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher
    )
    {
        this.unitOfWork = unitOfWork;
        this.userDomainRulesConfigProvider = userDomainRulesConfigProvider;
        this.userRepository = userRepository;
        this.passwordHasher = passwordHasher;
    }
    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetById(request.Id,cancellationToken);
        var hashedPassword = new HashedPassword(passwordHasher.Hash(request.password));
        user.ChangePassword(hashedPassword);
        await unitOfWork.Commit(cancellationToken);
        return Unit.Value;
    }
}