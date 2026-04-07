using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Users.API.Controllers;
using Users.Application.Commands;
using Users.Application.DTOs;
using Users.Domain;
using Users.Domain.Exceptions;
using Users.Infrastructure.Configuration;
using Microsoft.AspNetCore.Http;
using Users.Domain.Rules;
using Users.Domain.ValueObjects;
using Users.Domain.Aggregates;
using Users.Application.Querys;

namespace Tests.Unit.Users.API.Controllers;

public enum LoginUserResult
{
    UserNotFound = 0,
    WrongUsernameOrPassword = 1,
    Success = 2
}

public class UsersControllerTests
{
    private readonly UserNameDomainRules rules = new UserNameDomainRules(6, 10);
    private readonly IMediator mediator;
    private readonly IOptions<JwtConfigOptions> jwtConfigOptions;
    private readonly UsersController controller;
    public UsersControllerTests()
    {
        mediator = Substitute.For<IMediator>();
        jwtConfigOptions = Options.Create(new JwtConfigOptions
        {
            ExpiresMinutes = 30
        });
        controller = new UsersController(mediator, jwtConfigOptions);
    }
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TestCreateUser(bool expected)
    {
        var username = "yonyuk";
        var email = "user@gmail.com";
        var password = "yony01uk";
        var role = UserRole.User;

        var action = async () =>
        {
            var data = new RegisterUserDTO(username, email, password, role);
            var command = new RegisterUserCommand(data);
            return await controller.CreateUser(command);
        };
    }

    [Theory]
    [InlineData(LoginUserResult.Success)]
    [InlineData(LoginUserResult.WrongUsernameOrPassword)]
    [InlineData(LoginUserResult.UserNotFound)]
    public async Task TestLogin(LoginUserResult expected)
    {
        var username = "yonyuk";
        var password = "yony01uk";

        var data = new LoginUserDTO(username, password);

        switch (expected)
        {
            case LoginUserResult.UserNotFound:
                mediator.Send(Arg.Any<LoginUserCommand>(), Arg.Any<CancellationToken>())
                    .ThrowsAsync(new UserNotFoundException("username", username));
                break;

            case LoginUserResult.WrongUsernameOrPassword:
                mediator.Send(Arg.Any<LoginUserCommand>(), Arg.Any<CancellationToken>())
                    .Returns(new LoginResponseDTO(null, "Incorrect username or password"));
                break;

            default:
                mediator.Send(Arg.Any<LoginUserCommand>(), Arg.Any<CancellationToken>())
                    .Returns(new LoginResponseDTO("token", null));
                break;
        }

        var context = new DefaultHttpContext();
        controller.ControllerContext = new ControllerContext { HttpContext = context };

        var action = async () => await controller.Login(data);

        switch (expected)
        {
            case LoginUserResult.UserNotFound:
                await action.Should().ThrowAsync<UserNotFoundException>();
                break;
            case LoginUserResult.WrongUsernameOrPassword:
                var result1 = await action();
                result1.Should().BeOfType<UnauthorizedObjectResult>();
                break;

            default:
                var result2 = await action();
                await mediator.Received(1).Send(Arg.Any<LoginUserCommand>(), Arg.Any<CancellationToken>());
                result2.Should().BeOfType<AcceptedResult>();
                context.Response.Headers["Set-Cookie"].ToString().Should().Contain("access_token=token");
                break;
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TestGetUserById(bool exists)
    {

        if (exists)
        {
            var username = new UserName("yonyuk", rules);
            var email = new Email("user@gmail.com");
            var hash = new HashedPassword("hash");
            var user = new User(username, email, hash);

            mediator.Send(Arg.Any<GetUserByIdQuery>(), Arg.Any<CancellationToken>())
                .Returns(new UserDTO(user.Id, user.UserName.Value, user.Email.Value, user.Role));
        }
        else
            mediator.Send(Arg.Any<GetUserByIdQuery>(), Arg.Any<CancellationToken>())
                .ThrowsAsync(new UserNotFoundException("id", "id"));

        var action = async () => await controller.GetUserById(Guid.NewGuid());

        if (exists)
        {
            var result = await action();
            result.Should().BeOfType<OkObjectResult>();
            await mediator.Received(1).Send(Arg.Any<GetUserByIdQuery>(), Arg.Any<CancellationToken>());
        }
        else
            await action.Should().ThrowAsync<UserNotFoundException>();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TestGetUserByEmail(bool exists)
    {
        var email = new Email("user@gmail.com");

        if (exists)
        {
            var username = new UserName("yonyuk", rules);
            var hash = new HashedPassword("hash");
            var user = new User(username, email, hash);

            mediator.Send(Arg.Any<GetUserByEmailQuery>(), Arg.Any<CancellationToken>())
                .Returns(new UserDTO(user.Id, user.UserName.Value, user.Email.Value, user.Role));
        }
        else
            mediator.Send(Arg.Any<GetUserByEmailQuery>(), Arg.Any<CancellationToken>())
                .ThrowsAsync(new UserNotFoundException("email", email.Value));

        var action = async () => await controller.GetUserByEmail(email.Value);

        if (exists)
        {
            var result = await action();
            result.Should().BeOfType<OkObjectResult>();
            await mediator.Received(1).Send(Arg.Any<GetUserByEmailQuery>(), Arg.Any<CancellationToken>());
        }
        else
            await action.Should().ThrowAsync<UserNotFoundException>();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TestGetUserByName(bool exists)
    {
        var username = new UserName("yonyuk", rules);

        if (exists)
        {
            var email = new Email("user@gmail.com");
            var hash = new HashedPassword("hash");
            var user = new User(username, email, hash);

            mediator.Send(Arg.Any<GetUserByUserNameQuery>(), Arg.Any<CancellationToken>())
                .Returns(new UserDTO(user.Id, user.UserName.Value, user.Email.Value, user.Role));
        }
        else
            mediator.Send(Arg.Any<GetUserByUserNameQuery>(), Arg.Any<CancellationToken>())
                .ThrowsAsync(new UserNotFoundException("username", username.Value));

        var action = async () => await controller.GetUserByName(username.Value);

        if (exists)
        {
            var result = await action();
            result.Should().BeOfType<OkObjectResult>();
            await mediator.Received(1).Send(Arg.Any<GetUserByUserNameQuery>(), Arg.Any<CancellationToken>());
        }
        else
            await action.Should().ThrowAsync<UserNotFoundException>();
    }
}