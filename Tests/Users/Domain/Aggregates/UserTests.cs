using FluentAssertions;
using Users.Domain;
using Users.Domain.Aggregates;
using Users.Domain.Events;
using Users.Domain.Rules;
using Users.Domain.ValueObjects;

namespace Tests.Unit.Users.Domain.Aggregates;

public class UserTests
{
<<<<<<< HEAD
<<<<<<< HEAD
    private readonly UserNameDomainRules rules = new UserNameDomainRules(6, 10);

=======
    private readonly UserNameDomainRules rules = new UserNameDomainRules(6,10);
    
>>>>>>> f4f0ea9 (adds UserTests)
=======
    private readonly UserNameDomainRules rules = new UserNameDomainRules(6, 10);

>>>>>>> f74ea6d (adds argument null checking to UserTests)
    [Theory]
    [InlineData(UserRole.User)]
    [InlineData(UserRole.Host)]
    public void TestCreateUser(UserRole role)
    {
<<<<<<< HEAD
<<<<<<< HEAD
        var username = new UserName("yonyuk", rules);
        var email = new Email("user@gmail.com");
        var hashedPassword = new HashedPassword("asdgeweiusbj");

        var user = new User(username, email, hashedPassword, role);

=======
        var username = new UserName("yonyuk",rules);
        var email = new Email("user@gmail.com");
        var hashedPassword = new HashedPassword("asdgeweiusbj");

        var user = new User(username,email,hashedPassword,role);
        
>>>>>>> f4f0ea9 (adds UserTests)
=======
        var username = new UserName("yonyuk", rules);
        var email = new Email("user@gmail.com");
        var hashedPassword = new HashedPassword("asdgeweiusbj");

        var user = new User(username, email, hashedPassword, role);

>>>>>>> f74ea6d (adds argument null checking to UserTests)
        user.Id.Should().NotBe(Guid.Empty);
        user.UserName.Should().Be(username);
        user.Email.Should().Be(email);
        user.HashedPassword.Should().Be(hashedPassword);
        user.Role.Should().Be(role);

        user.Events.Should().ContainSingle()
            .Which.Should().BeOfType<UserRegisteredEvent>()
<<<<<<< HEAD
<<<<<<< HEAD
            .And.Match<UserRegisteredEvent>(e =>
=======
            .And.Match<UserRegisteredEvent>(e => 
>>>>>>> f4f0ea9 (adds UserTests)
=======
            .And.Match<UserRegisteredEvent>(e =>
>>>>>>> f74ea6d (adds argument null checking to UserTests)
                e.Id == user.Id &&
                e.UserName == user.UserName &&
                e.Email == user.Email &&
                e.Role == user.Role);
    }
<<<<<<< HEAD
<<<<<<< HEAD

=======
    
>>>>>>> f4f0ea9 (adds UserTests)
=======

>>>>>>> f74ea6d (adds argument null checking to UserTests)
    [Theory]
    [InlineData(UserRole.User)]
    [InlineData(UserRole.Host)]
    public void TestInternalCreateUser(UserRole role)
    {
        var id = Guid.NewGuid();
<<<<<<< HEAD
<<<<<<< HEAD
        var username = new UserName("yonyuk", rules);
        var email = new Email("user@gmail.com");
        var hashedPassword = new HashedPassword("asdgeweiusbj");

        var user = new User(id, username, email, hashedPassword, role);

=======
        var username = new UserName("yonyuk",rules);
        var email = new Email("user@gmail.com");
        var hashedPassword = new HashedPassword("asdgeweiusbj");

        var user = new User(id,username,email,hashedPassword,role);
        
>>>>>>> f4f0ea9 (adds UserTests)
=======
        var username = new UserName("yonyuk", rules);
        var email = new Email("user@gmail.com");
        var hashedPassword = new HashedPassword("asdgeweiusbj");

        var user = new User(id, username, email, hashedPassword, role);

>>>>>>> f74ea6d (adds argument null checking to UserTests)
        user.Id.Should().Be(id);
        user.UserName.Should().Be(username);
        user.Email.Should().Be(email);
        user.HashedPassword.Should().Be(hashedPassword);
        user.Role.Should().Be(role);
    }
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> f74ea6d (adds argument null checking to UserTests)

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
        var username = new UserName("yonyuk",rules);
        var email = new Email("user@gmail.com");
        var hashedPassword = new HashedPassword("hash");
        var action1 = () => new User(null,email,hashedPassword);
        var action2 = () => new User(username,null,hashedPassword);
        var action3 = () => new User(username,email,null);

        action1.Should().Throw<ArgumentNullException>();
        action2.Should().Throw<ArgumentNullException>();
        action3.Should().Throw<ArgumentNullException>();
    }
<<<<<<< HEAD
=======
>>>>>>> f4f0ea9 (adds UserTests)
=======
>>>>>>> f74ea6d (adds argument null checking to UserTests)
}