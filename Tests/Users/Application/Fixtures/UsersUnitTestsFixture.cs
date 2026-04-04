
using AutoMapper;
using NSubstitute;
using Users.Application.DTOs;
using Users.Domain;
using Users.Domain.Aggregates;
using Users.Domain.Rules;
using Users.Domain.ValueObjects;

namespace Tests.Unit.Users.Fixtures;

public class UsersUnitTestsFixture : IAsyncLifetime
{
    public IEnumerable<User> Users { get; private set; }
    public IMapper MockedMapper { get; private set; }
    private readonly UserNameDomainRules rules = new UserNameDomainRules(6, 10);
    public UsersUnitTestsFixture()
    {
        Users = Populate("yonyuk", "jose01", "brayan", "cuervo", "nayeli");
        MockedMapper = Substitute.For<IMapper>();
        MockedMapper.Map<IEnumerable<UserDTO>>(Arg.Any<IEnumerable<User>>())
            .Returns(callInfo => callInfo.Arg<IEnumerable<User>>()
                    .Select(user => new UserDTO(
                        user.Id,
                        user.UserName.Value,
                        user.Email.Value,
                        user.Role
                    )
                )
            );
        MockedMapper.Map<UserDTO>(Arg.Any<User>())
            .Returns(
                callinfo =>
                {
                    var user = callinfo.Arg<User>();
                    return new UserDTO(
                        user.Id,
                        user.UserName.Value,
                        user.Email.Value,
                        user.Role
                    );
                }
            );
    }
    public Task DisposeAsync() => Task.CompletedTask;

    public async Task InitializeAsync()
    {
        // Users = Populate("yonyuk","jose01","brayan","cuervo","nayeli");
    }
    IEnumerable<User> Populate(params string[] names)
    {
        var role = 0;
        foreach (var name in names)
        {
            var username = new UserName(name, rules);
            var email = new Email($"{name}@gmail.com");
            var hashedPassword = new HashedPassword($"{name}_password_hashed");
            yield return new User(username, email, hashedPassword,(UserRole)role);
            role ^= 1;
        }
    }
}

[CollectionDefinition("Users Collection Fixture For Unit Testing On Application Layer")]
public class UsersUnitTestsFixtureShared : ICollectionFixture<UsersUnitTestsFixture> { }