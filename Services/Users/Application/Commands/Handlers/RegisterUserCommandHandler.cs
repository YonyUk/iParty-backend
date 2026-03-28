using Common.Domain;
using MediatR;
using Users.Application.DTOs;
using Users.Application.Services;
using Users.Domain;
using Users.Domain.Rules;
using Users.Domain.Services;
using Users.Domain.ValueObjects;

namespace Users.Application.Commands.Handlers;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserCreatedResponseDTO>
{
    private readonly IUserRepository userRepository;
    private readonly UserRegistrationService userRegistrationService;
    private readonly IPasswordHasher passwordHasher;
    private readonly IUnitOfWork unitOfWork;
    private readonly IUserDomainRulesConfigProvider userDomainRulesConfigProvider;
    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IUserUniquenessChecker userUniquenessChecker,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        IUserDomainRulesConfigProvider userDomainRulesConfigProvider
    )
    {
        this.userRepository = userRepository;
        userRegistrationService = new UserRegistrationService(userUniquenessChecker);
        this.passwordHasher = passwordHasher;
        this.unitOfWork = unitOfWork;
        this.userDomainRulesConfigProvider = userDomainRulesConfigProvider;
    }
    public async Task<UserCreatedResponseDTO> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var userData = request.data;
        // build the value objects
        var username = new UserName(userData.UserName,userDomainRulesConfigProvider.UserNameDomainRules);
        var email = new Email(userData.Email);
        var hashedPassword = new HashedPassword(passwordHasher.Hash(userData.Password));
        // registers the user
        var userCreated = await userRegistrationService.RegisterUser(username,email,hashedPassword,userData.Role);
        // persists the user
        var userId = await userRepository.Create(userCreated,cancellationToken);
        // save the operation
        await unitOfWork.Commit(cancellationToken);
        return new UserCreatedResponseDTO(userId);
    }
}