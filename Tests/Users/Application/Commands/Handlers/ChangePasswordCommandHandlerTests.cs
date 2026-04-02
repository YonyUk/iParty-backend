using Common.Domain;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Users.Application.Commands;
using Users.Application.Commands.Handlers;
using Users.Application.Services;
using Users.Domain;
using Users.Domain.Aggregates;
using Users.Domain.Exceptions;
using Users.Domain.Rules;
using Users.Domain.ValueObjects;

namespace Tests.Unit.Users.Application.Commands.Handlers;

public class ChangePasswordCommandHandlerTests
{
    private readonly IUserDomainRulesConfigProvider userDomainRulesConfigProvider;
    private readonly IUserRepository userRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IPasswordHasher passwordHasher;
    private readonly UserNameDomainRules rules = new UserNameDomainRules(6,10);
    public ChangePasswordCommandHandlerTests()
    {
        userRepository = Substitute.For<IUserRepository>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        passwordHasher = Substitute.For<IPasswordHasher>();
        userDomainRulesConfigProvider = Substitute.For<IUserDomainRulesConfigProvider>();
    }
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TestChangePassword(bool userExists)
    {
        var username = new UserName("yonyuk",rules);
        var email = new Email("test@gmail.com");
        var password = "new_password";
        passwordHasher.Hash(password).Returns($"{password}_hashed");
        var hashedPassword = new HashedPassword($"{password}_hashed");
        var user = new User(username,email,hashedPassword);
        if (userExists)
            userRepository.GetById(Arg.Any<Guid>()).Returns(user);
        else
            userRepository.GetById(Arg.Any<Guid>()).Throws(new UserNotFoundException("id","id"));
        
        var command = new ChangePasswordCommand(Guid.NewGuid(),password);
        var handler = new ChangePasswordCommandHandler(userDomainRulesConfigProvider,userRepository,unitOfWork,passwordHasher);
        
        var action = async () =>
        {
            await handler.Handle(command,CancellationToken.None);
            await userRepository.Received(1).GetById(Arg.Any<Guid>());
            passwordHasher.Received(1).Hash(password);
            await unitOfWork.Received(1).Commit(Arg.Any<CancellationToken>());
        };

        if (userExists)
            await action.Should().NotThrowAsync();
        else
            await action.Should().ThrowAsync<UserNotFoundException>();
    }
}