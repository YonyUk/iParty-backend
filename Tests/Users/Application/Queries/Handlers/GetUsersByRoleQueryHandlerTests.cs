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
public class GetUsersByRoleQueryHandlerTests
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    private readonly IEnumerable<User> users;
    public GetUsersByRoleQueryHandlerTests(UsersUnitTestsFixture fixture)
    {
        userRepository = Substitute.For<IUserRepository>();
        mapper = fixture.MockedMapper;
        users = fixture.Users;
    }

    [Theory]
    [InlineData(UserRole.User)]
    [InlineData(UserRole.Host)]
    public async Task TestGetUsersByRole(UserRole role)
    {
        var expectedUsers = users.Where(user => user.Role == role);
        userRepository.GetUsersByRole(role,Arg.Any<CancellationToken>()).Returns(expectedUsers);

        var query = new GetUsersByRoleQuery(role);
        var handler = new GetUsersByRoleQueryHandler(userRepository,mapper);

        var usersResult = await handler.Handle(query,CancellationToken.None);
        
        await userRepository.Received(1).GetUsersByRole(role,Arg.Any<CancellationToken>());
        mapper.Received(1).Map<IEnumerable<UserDTO>>(expectedUsers);
        
        usersResult.Should().OnlyContain(userDTO => userDTO.Role == role);
        usersResult.Should().HaveCount(expectedUsers.Count());
    }
}