using FluentAssertions;
using Users.Domain;
using Users.Domain.Aggregates;
using Users.Domain.Events;
using Users.Domain.Rules;
using Users.Domain.ValueObjects;

namespace Tests.Unit.Users.Domain.Aggregates;

public class UserTests
{
    private readonly UserNameDomainRules rules = new UserNameDomainRules(6, 10);

    [Theory]
    [InlineData(UserRole.User)]
    [InlineData(UserRole.Host)]
    public void TestCreateUser(UserRole role)
    {
        var username = new UserName("yonyuk", rules);
        var email = new Email("user@gmail.com");
        var hashedPassword = new HashedPassword("asdgeweiusbj");

        var user = new User(username, email, hashedPassword, role);

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
        var username = new UserName("yonyuk", rules);
        var email = new Email("user@gmail.com");
        var hashedPassword = new HashedPassword("asdgeweiusbj");

        var user = new User(id, username, email, hashedPassword, role);

        user.Id.Should().Be(id);
        user.UserName.Should().Be(username);
        user.Email.Should().Be(email);
        user.HashedPassword.Should().Be(hashedPassword);
        user.Role.Should().Be(role);
    }

    [Fact]
    public void TestChangeUserPassword()
    {
        // Arrange
        var user = new User(
            new UserName("johndoe", rules),
            new Email("john@example.com"),
            new HashedPassword("oldHash"),
            UserRole.User
        );
        user.ClearEvents();

        var newPassword = new HashedPassword("newHash");

        user.ChangePassword(newPassword);

        user.HashedPassword.Should().Be(newPassword);

        user.Events.Should().ContainSingle()
            .Which.Should().BeOfType<UserPasswordChangedEvent>()
            .And.Match<UserPasswordChangedEvent>(e =>
                e.Id == user.Id &&
                e.HashedPassword == newPassword);
    }

    [Fact]
    public void TestCreateUserWithNullArgument()
    {
        var username = new UserName("yonyuk", rules);
        var email = new Email("user@gmail.com");
        var hashedPassword = new HashedPassword("hash");
        var action1 = () => new User(null, email, hashedPassword);
        var action2 = () => new User(username, null, hashedPassword);
        var action3 = () => new User(username, email, null);

        action1.Should().Throw<ArgumentNullException>();
        action2.Should().Throw<ArgumentNullException>();
        action3.Should().Throw<ArgumentNullException>();
    }
}