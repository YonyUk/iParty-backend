using MediatR;
using Users.Application.DTOs;
using Users.Application.Services;
using Users.Domain;
using Users.Domain.Rules;
using Users.Domain.ValueObjects;

namespace Users.Application.Commands.Handlers;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponseDTO>
{
    private readonly IUserRepository userRepository;
    private readonly IPasswordHasher passwordHasher;
    private readonly IUserDomainRulesConfigProvider userDomainRulesConfigProvider;
    private readonly IUserAuthenticator userAuthenticator;
    public LoginUserCommandHandler
    (
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUserDomainRulesConfigProvider userDomainRulesConfigProvider,
        IUserAuthenticator userAuthenticator
    )
    {
        this.userRepository = userRepository;
        this.passwordHasher = passwordHasher;
        this.userDomainRulesConfigProvider = userDomainRulesConfigProvider;
        this.userAuthenticator = userAuthenticator;
    }
    public async Task<LoginResponseDTO> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var username = new UserName(request.data.UserName,userDomainRulesConfigProvider.UserNameDomainRules);
        var user = await userRepository.GetByName(username,cancellationToken);
        if (!passwordHasher.Verify(request.data.Password,user.HashedPassword.Value))
            return new LoginResponseDTO(null,"Incorrect username or password");
        return new LoginResponseDTO(userAuthenticator.Authenticate(user),null);
    }
}