using FluentAssertions;
using Users.Domain;
using Users.Domain.Aggregates;
using Users.Domain.Events;
using Users.Domain.Rules;
using Users.Domain.ValueObjects;

namespace Tests.Unit.Users.Domain.Aggregates;

public class UserTests
{
    private readonly UserNameDomainRules rules = new UserNameDomainRules(6,10);
    
    [Theory]
    [InlineData(UserRole.User)]
    [InlineData(UserRole.Host)]
    public void TestCreateUser(UserRole role)
    {
        var username = new UserName("yonyuk",rules);
        var email = new Email("user@gmail.com");
        var hashedPassword = new HashedPassword("asdgeweiusbj");

        var user = new User(username,email,hashedPassword,role);
        
        user.Id.Should().NotBe(Guid.Empty);
        user.UserName.Should().Be(username);
        user.Email.Should().Be(email);
        user.HashedPassword.Should().Be(hashedPassword);
        user.Role.Should().Be(role);

        user.Events.Should().ContainSingle()
            .Which.Should().BeOfType<UserRegisteredEvent>()
            .And.Match<UserRegisteredEvent>(e => 
                e.Id == user.Id &&
                e.UserName == user.UserName &&
                e.Email == user.Email &&
                e.Role == user.Role);
    }
    
    [Theory]
    [InlineData(UserRole.User)]
    [InlineData(UserRole.Host)]
    public void TestInternalCreateUser(UserRole role)
    {
        var id = Guid.NewGuid();
        var username = new UserName("yonyuk",rules);
        var email = new Email("user@gmail.com");
        var hashedPassword = new HashedPassword("asdgeweiusbj");

        var user = new User(id,username,email,hashedPassword,role);
        
        user.Id.Should().Be(id);
        user.UserName.Should().Be(username);
        user.Email.Should().Be(email);
        user.HashedPassword.Should().Be(hashedPassword);
        user.Role.Should().Be(role);
    }
}