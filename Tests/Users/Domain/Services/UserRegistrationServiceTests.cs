using FluentAssertions;
using NSubstitute;
using Users.Domain.Exceptions;
using Users.Domain.Rules;
using Users.Domain.Services;
using Users.Domain.ValueObjects;

namespace Tests.Unit.Users.Domain.Services;

public class UserRegistrationServiceTests
{
    private readonly IUserUniquenessChecker userUniquenessChecker;
    private readonly UserRegistrationService userRegistrationService;
    private readonly UserNameDomainRules userNameDomainRules = new UserNameDomainRules(6,10);
    public UserRegistrationServiceTests()
    {
        userUniquenessChecker = Substitute.For<IUserUniquenessChecker>();
        userRegistrationService = new UserRegistrationService(userUniquenessChecker);
    }
    [Theory]
    [InlineData(true,true)]
    [InlineData(false,true)]
    [InlineData(true,false)]
    [InlineData(false,false)]
    public async Task TestUserRegistrationService(bool usernameUnique,bool emailUnique)
    {
        var username = new UserName("yonyuk",userNameDomainRules);
        var email = new Email("user@gmail.com");
        var hash = new HashedPassword("hash");

        userUniquenessChecker.IsUnique(username).Returns(usernameUnique);
        userUniquenessChecker.IsUnique(email).Returns(emailUnique);

        var action = async () =>
        {
            var user =await userRegistrationService.RegisterUser(username,email,hash);
            user.Should().NotBeNull();
            user.Id.Should().NotBe(Guid.Empty);
            user.UserName.Should().Be(username);
            user.Email.Should().Be(email);
            user.HashedPassword.Should().Be(hash);

            await userUniquenessChecker.Received(1).IsUnique(username);
            await userUniquenessChecker.Received(1).IsUnique(email);
        };

        if (usernameUnique && emailUnique)
            await action.Should().NotThrowAsync();
        if (!(usernameUnique & emailUnique))
            await action.Should().ThrowAsync<UserAlreadyExistsException>();
        if (!usernameUnique)
            await userUniquenessChecker.DidNotReceive().IsUnique(email);
    }
}