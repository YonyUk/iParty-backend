using Common.Domain;
using FluentAssertions;
using NSubstitute;
using Users.Application.Commands;
using Users.Application.Commands.Handlers;
using Users.Application.DTOs;
using Users.Application.Services;
using Users.Domain;
using Users.Domain.Aggregates;
using Users.Domain.Exceptions;
using Users.Domain.Rules;
using Users.Domain.Services;
using Users.Domain.ValueObjects;

namespace Tests.Unit.Users.Application.Commands.Handlers;

public enum TestCreateUserExpectedValue
{
    OK = 0,
    UserNameAlreadyExists = 1,
    EmailAlreadyExists = 2,
    UserNameInvalid = 3,
    EmailInvalid = 4
}
public class RegisterUserCommandHandlerTests
{
    private readonly IUserRepository userRepository;
    private readonly IUserUniquenessChecker userUniquenessChecker;
    private readonly IPasswordHasher passwordHasher;
    private readonly IUnitOfWork unitOfWork;
    private readonly IUserDomainRulesConfigProvider userDomainRulesConfigProvider;
    private readonly UserNameDomainRules userNameRules = new UserNameDomainRules(6, 10);
    public RegisterUserCommandHandlerTests()
    {
        userRepository = Substitute.For<IUserRepository>();
        userUniquenessChecker = Substitute.For<IUserUniquenessChecker>();
        passwordHasher = Substitute.For<IPasswordHasher>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        userDomainRulesConfigProvider = Substitute.For<IUserDomainRulesConfigProvider>();
    }

    [Theory]
    [InlineData("yonyuk", "user@gmail.com", "yony01uk", UserRole.User, TestCreateUserExpectedValue.OK)]
    [InlineData("yonyuk", "user@gmail.com", "yony01uk", UserRole.Host, TestCreateUserExpectedValue.OK)]
    [InlineData("yony", "user@gmail.com", "yony01uk", UserRole.User, TestCreateUserExpectedValue.UserNameInvalid)]
    [InlineData("yonyuk", "usergmail.com", "yony01uk", UserRole.User, TestCreateUserExpectedValue.EmailInvalid)]
    [InlineData("yonyuk", "user@gmail.com", "yony01uk", UserRole.User, TestCreateUserExpectedValue.EmailAlreadyExists)]
    [InlineData("yonyuk", "user@gmail.com", "yony01uk", UserRole.User, TestCreateUserExpectedValue.UserNameAlreadyExists)]
    [InlineData(null, "user@gmail.com", "yony01uk", UserRole.User, TestCreateUserExpectedValue.UserNameInvalid)]
    [InlineData("     ", "user@gmail.com", "yony01uk", UserRole.User, TestCreateUserExpectedValue.UserNameInvalid)]
    [InlineData("yonyuk", null, "yony01uk", UserRole.User, TestCreateUserExpectedValue.EmailInvalid)]
    [InlineData("yonyuk", "     ", "yony01uk", UserRole.User, TestCreateUserExpectedValue.EmailInvalid)]
    public async Task TestCreateUser(
        string username,
        string email,
        string password,
        UserRole role,
        TestCreateUserExpectedValue expected
    )
    {
        var data = new RegisterUserDTO(username, email, password, role);
        var command = new RegisterUserCommand(data);
        userDomainRulesConfigProvider.UserNameDomainRules.Returns(userNameRules);
        passwordHasher.Hash(password).Returns($"{password}_hashed");

        Guid expectedId = Guid.NewGuid();
        UserName usernameObject = null;
        Email emailObject = null;
        var hashedPassword = new HashedPassword(passwordHasher.Hash(password));

        if (expected != TestCreateUserExpectedValue.UserNameInvalid)
            usernameObject = new UserName(username, userNameRules);
        if (expected != TestCreateUserExpectedValue.EmailInvalid)
            emailObject = new Email(email);

        if (expected != TestCreateUserExpectedValue.UserNameInvalid && expected != TestCreateUserExpectedValue.EmailInvalid)
        {
            userUniquenessChecker.IsUnique(usernameObject!).Returns(expected != TestCreateUserExpectedValue.UserNameAlreadyExists ? true : false);
            userUniquenessChecker.IsUnique(emailObject!).Returns(expected != TestCreateUserExpectedValue.EmailAlreadyExists ? true : false);
        }

        if (expected == TestCreateUserExpectedValue.OK)
            userRepository.Create(Arg.Any<User>()).Returns(expectedId);

        var handler = new RegisterUserCommandHandler(userRepository, userUniquenessChecker, passwordHasher, unitOfWork, userDomainRulesConfigProvider);

        var action = async () =>
        {
            var response = await handler.Handle(command, CancellationToken.None);
            await userUniquenessChecker.Received(1).IsUnique(usernameObject!);
            await userUniquenessChecker.Received(1).IsUnique(emailObject!);
            await userRepository.Received(1).Create(Arg.Any<User>());
            response.id.Should().Be(expectedId);
        };

        switch (expected)
        {
            case TestCreateUserExpectedValue.UserNameInvalid:
                await action.Should().ThrowAsync<InvalidUserNameException>();
                break;

            case TestCreateUserExpectedValue.EmailInvalid:
                await action.Should().ThrowAsync<InvalidEmailException>();
                break;

            case TestCreateUserExpectedValue.UserNameAlreadyExists:
                await action.Should().ThrowAsync<UserAlreadyExistsException>();
                break;

            case TestCreateUserExpectedValue.EmailAlreadyExists:
                await action.Should().ThrowAsync<UserAlreadyExistsException>();
                break;

            default:
                await action.Should().NotThrowAsync();
                break;
        }
    }
}