using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Users.Application.Commands;
using Users.Application.Commands.Handlers;
using Users.Application.DTOs;
using Users.Application.Services;
using Users.Domain;
using Users.Domain.Aggregates;
using Users.Domain.Exceptions;
using Users.Domain.Rules;
using Users.Domain.ValueObjects;

namespace Tests.Unit.Users.Application.Commands.Handlers;

public enum TestLoginUserExpectedValue
{
    OK = 0,
    UserNameInvalid = 1,
    IncorrectPassword = 2,
    UserNotFound = 3
}
public class LoginUserCommandHandlerTests
{
    private readonly IUserRepository userRepository;
    private readonly IPasswordHasher passwordHasher;
    private readonly IUserDomainRulesConfigProvider userDomainRulesConfigProvider;
    private readonly IUserAuthenticator userAuthenticator;
    private readonly UserNameDomainRules rules = new UserNameDomainRules(6, 10);
    public LoginUserCommandHandlerTests()
    {
        userRepository = Substitute.For<IUserRepository>();
        passwordHasher = Substitute.For<IPasswordHasher>();
        userDomainRulesConfigProvider = Substitute.For<IUserDomainRulesConfigProvider>();
        userAuthenticator = Substitute.For<IUserAuthenticator>();
    }
    [Theory]
    [InlineData("yonyuk", "yony01uk", TestLoginUserExpectedValue.OK)]
    [InlineData("yonyuk", "yony01uk", TestLoginUserExpectedValue.UserNotFound)]
    [InlineData("yony", "yonyuk", TestLoginUserExpectedValue.UserNameInvalid)]
    [InlineData("yonyuk", "wrongpassword", TestLoginUserExpectedValue.IncorrectPassword)]
    public async Task TestLoginUserCommandHandler(string username, string password, TestLoginUserExpectedValue expected)
    {
        userDomainRulesConfigProvider.UserNameDomainRules.Returns(rules);
        passwordHasher.Hash(password).Returns($"{password}_hashed");
        UserName userName = null;
        var email = new Email("test@gmail.com");
        var hashedPassword = new HashedPassword(passwordHasher.Hash(password));
        if (expected != TestLoginUserExpectedValue.UserNameInvalid)
        {
            userName = new UserName(username, userDomainRulesConfigProvider.UserNameDomainRules);
            var user = new User(userName, email, hashedPassword);
            if (expected != TestLoginUserExpectedValue.UserNotFound)
                userRepository.GetByName(userName).Returns(user);
            else
                userRepository.GetByName(userName).Throws(new UserNotFoundException("UserName", username));
        }
        passwordHasher.Verify(password, hashedPassword.Value).Returns(expected != TestLoginUserExpectedValue.IncorrectPassword ? true : false);
        if (expected == TestLoginUserExpectedValue.OK)
            userAuthenticator.Authenticate(Arg.Any<User>()).Returns("jwt acces token");

        var dto = new LoginUserDTO(username, password);
        var command = new LoginUserCommand(dto);
        var handler = new LoginUserCommandHandler(userRepository, passwordHasher, userDomainRulesConfigProvider, userAuthenticator);

        var action = async () =>
        {
            var response = await handler.Handle(command, CancellationToken.None);
            await userRepository.Received(1).GetByName(userName!);
            passwordHasher.Received(1).Verify(password, hashedPassword.Value);
            if (expected == TestLoginUserExpectedValue.OK)
                userAuthenticator.Received(1).Authenticate(Arg.Any<User>());
            return response;
        };

        switch (expected)
        {
            case TestLoginUserExpectedValue.UserNameInvalid:
                await action.Should().ThrowAsync<InvalidUserNameException>();
                break;

            case TestLoginUserExpectedValue.UserNotFound:
                await action.Should().ThrowAsync<UserNotFoundException>();
                break;

            case TestLoginUserExpectedValue.IncorrectPassword:
                var response = await action();
                response.token.Should().BeNull();
                break;

            default:
                await action.Should().NotThrowAsync();
                break;
        }
    }
}