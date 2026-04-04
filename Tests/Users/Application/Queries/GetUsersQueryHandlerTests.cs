using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Tests.Unit.Users.Fixtures;
using Users.Application.DTOs;
using Users.Application.Querys;
using Users.Application.Querys.Handlers;
using Users.Domain;
using Users.Domain.Aggregates;

namespace Tests.Unit.Users.Application.Queries.Handlers;

[Collection("Users Collection Fixture For Unit Testing On Application Layer")]
public class GetUsersQueryHandlerTests
{
    private readonly IEnumerable<User> users;
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    public GetUsersQueryHandlerTests(UsersUnitTestsFixture fixture)
    {
        userRepository = Substitute.For<IUserRepository>();
        mapper = fixture.MockedMapper;
        users = fixture.Users;
    }

    [Fact]
    public async Task TestGetUsers()
    {
        userRepository.GetUsers(Arg.Any<CancellationToken>()).Returns(users);
        
        var query = new GetUsersQuery();
        var handler = new GetUsersQueryHandler(userRepository,mapper);

        var usersResult = await handler.Handle(query,CancellationToken.None);
        await userRepository.Received(1).GetUsers(Arg.Any<CancellationToken>());
        mapper.Received(1).Map<IEnumerable<UserDTO>>(users);
        usersResult.Should().HaveCount(users.Count());
    }
}