using AutoMapper;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tests.Unit.Users.Fixtures;
using Users.Application.DTOs;
using Users.Application.Querys;
using Users.Application.Querys.Handlers;
using Users.Domain;
using Users.Domain.Aggregates;
using Users.Domain.Exceptions;
using Users.Domain.Rules;
using Users.Domain.ValueObjects;

namespace Tests.Unit.Users.Application.Queries.Handlers;

[Collection("Users Collection Fixture For Unit Testing On Application Layer")]
public class GetUserByEmailQueryHandlerTests
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    private readonly UserNameDomainRules rules = new UserNameDomainRules(6, 10);
    private readonly User user;
    private readonly UserDTO userExpected;
    public GetUserByEmailQueryHandlerTests(UsersUnitTestsFixture fixture)
    {
        userRepository = Substitute.For<IUserRepository>();
        mapper = fixture.MockedMapper;
        var username = new UserName("yonyuk",rules);
        var email = new Email("user@gmail.com");
        var hashedPassword = new HashedPassword("hash");
        user = new User(username,email,hashedPassword);
        userExpected = new UserDTO(user.Id,username.Value,email.Value,user.Role);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TestGetUserByEmail(bool exists)
    {
        if (exists)
            userRepository.GetByEmail(user.Email,Arg.Any<CancellationToken>()).Returns(user);
        else
            userRepository.GetByEmail(Arg.Any<Email>(),Arg.Any<CancellationToken>())
                .ThrowsAsync(new UserNotFoundException("Email",user.Email.Value));

        var query = new GetUserByEmailQuery(user.Email.Value);
        var handler = new GetUserByEmailQueryHandler(userRepository,mapper);

        var action = async () =>
        {
            var userResult = await handler.Handle(query,CancellationToken.None);
            await userRepository.Received(1).GetByEmail(user.Email,Arg.Any<CancellationToken>());
            mapper.Received(1).Map<UserDTO>(user);
            userResult.Should().Be(userExpected);
        };

        if (!exists)
            await action.Should().ThrowAsync<UserNotFoundException>();
        else
            await action.Should().NotThrowAsync();
    }
}