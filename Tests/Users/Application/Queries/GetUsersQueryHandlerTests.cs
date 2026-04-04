using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Users.Application.DTOs;
using Users.Application.Querys;
using Users.Application.Querys.Handlers;
using Users.Domain;
using Users.Domain.Aggregates;
using Users.Domain.Rules;
using Users.Domain.ValueObjects;

namespace Tests.Unit.Users.Application.Queries.Handlers;

public class GetUsersQueryHandlerTests
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    private readonly IEnumerable<User> users;
    private readonly UserNameDomainRules rules = new UserNameDomainRules(6,10);
    public GetUsersQueryHandlerTests()
    {
        userRepository = Substitute.For<IUserRepository>();
        mapper = Substitute.For<IMapper>();
        users = Populate("yonyuk","jose01","brayan","cuervo","nayeli");
    }
    IEnumerable<User> Populate(params string[] names)
    {
        foreach(var name in names)
        {
            var username = new UserName(name,rules);
            var email = new Email($"{name}@gmail.com");
            var hashedPassword = new HashedPassword($"{name}_password_hashed");
            yield return new User(username,email,hashedPassword);
        }
    }

    [Fact]
    public async Task TestGetUsers()
    {
        userRepository.GetUsers(Arg.Any<CancellationToken>()).Returns(users);
        var query = new GetUsersQuery();
        var handler = new GetUsersQueryHandler(userRepository,mapper);

        var action = async () =>
        {
            var users = await handler.Handle(query,CancellationToken.None);
            await userRepository.Received(1).GetUsers(Arg.Any<CancellationToken>());
            mapper.Received(1).Map<IEnumerable<UserDTO>>(this.users);
        };

        await action.Should().NotThrowAsync();
    }
}