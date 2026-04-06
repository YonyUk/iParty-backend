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

namespace Tests.Unit.Users.API.Controllers;

public class UsersControllerTests
{
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
        controller = new UsersController(mediator,jwtConfigOptions);
    }
    [Theory]
    [InlineData("yonyuk","user@gmail.com","yony01uk",UserRole.User,true)]
    [InlineData("yonyuk","user@gmail.com","yony01uk",UserRole.User,false)]
    public async Task TestCreateUser(string username,string email,string password,UserRole role,bool success)
    {
        if (!success)
            mediator.Send(Arg.Any<RegisterUserCommand>(),Arg.Any<CancellationToken>())
                .Throws(new UserAlreadyExistsException("username",username));
        var action = async () =>
        {  
            var data = new RegisterUserDTO(username,email,password,role);
            var command = new RegisterUserCommand(data);
            return await controller.CreateUser(command);
        };
        if (success)
        {
            await action.Should().NotThrowAsync();
            await mediator.Received(1).Send(Arg.Any<RegisterUserCommand>(),Arg.Any<CancellationToken>());
        }
        else
            await action.Should().ThrowAsync<UserAlreadyExistsException>();
    }
}