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
public class GetUserByIdQueryHandlerTests
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    private readonly UserNameDomainRules rules = new UserNameDomainRules(6, 10);
    private readonly User user;
    private readonly UserDTO userExpected;
    public GetUserByIdQueryHandlerTests(UsersUnitTestsFixture fixture)
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
    public async Task TestGetUserById(bool exists)
    {
        if (exists)
            userRepository.GetById(user.Id,Arg.Any<CancellationToken>()).Returns(user);
        else
            userRepository.GetById(Arg.Any<Guid>(),Arg.Any<CancellationToken>())
                .ThrowsAsync(new UserNotFoundException("Id",user.Id.ToString()));

        var query = new GetUserByIdQuery(user.Id);
        var handler = new GetUserByIdQueryHandler(userRepository,mapper);

        var action = async () =>
        {
            var userResult = await handler.Handle(query,CancellationToken.None);
            await userRepository.Received(1).GetById(user.Id,Arg.Any<CancellationToken>());
            mapper.Received(1).Map<UserDTO>(user);
            userResult.Should().Be(userExpected);
        };

        if (!exists)
            await action.Should().ThrowAsync<UserNotFoundException>();
        else
            await action.Should().NotThrowAsync();
    }
}