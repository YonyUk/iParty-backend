using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Users.Domain.Aggregates;
using Users.Domain.Rules;
using Users.Domain.ValueObjects;
using Users.Infrastructure.Configuration;
using Users.Infrastructure.Services.Security;

namespace Tests.Unit.Users.Infrastructure.Persistence.Services;

public class UserAuthenticatorTests
{
    private readonly UserNameDomainRules rules = new UserNameDomainRules(6,10);

    [Fact]
    public void TestAuthenticate()
    {
        var options = Options.Create(new JwtConfigOptions
        {
            SecretKey = "mysupersecretkeyforauthenticationonthistestapp1234!intestingmodeforxunitprojectontests",
            Issuer = "test",
            Audience = "test",
            ExpiresMinutes = 5
        });

        var authenticator = new UserAuthenticator(options);
        var username = new UserName("yonyuk",rules);
        var email = new Email("user@gmail.com");
        var hashedPassword = new HashedPassword("hash");
        var user = new User(username,email,hashedPassword);

        string token = authenticator.Authenticate(user);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        
        jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value.Should().Be(user.Id.ToString());
        jwt.Claims.First(c => c.Type == ClaimTypes.Email).Value.Should().Be(user.Email.Value);
        jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value.Should().Be(user.Role.ToString());
        jwt.Issuer.Should().Be("test");
        jwt.Audiences.Should().Contain( e => e == "test");
        var expirationTime = jwt.ValidTo;
        var expiresAt = DateTime.UtcNow.AddMinutes(options.Value.ExpiresMinutes);
        expirationTime.Should().BeCloseTo(expiresAt,TimeSpan.FromSeconds(2));
    }
}