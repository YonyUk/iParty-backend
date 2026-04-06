using FluentAssertions;
using Users.Infrastructure.Services.Security;

namespace Tests.Unit.Users.Infrastructure.Persistence.Services;

public class PasswordHasherTests
{
    private readonly PasswordHasher passwordHasher = new PasswordHasher();

    [Fact]
    public void TestPasswordHash()
    {
        string hash = passwordHasher.Hash("password");
        hash.Should().NotBeEmpty();
    }

    [Fact]
    public void TestPasswordHashGeneration()
    {
        string hash1 = passwordHasher.Hash("password");
        string hash2 = passwordHasher.Hash("password");
        hash1.Should().NotBe(hash2);
    }
    [Theory]
    [InlineData("password","password")]
    [InlineData("password1","password2")]
    public void TestPasswordHashVerify(string password1,string password2)
    {
        string hash = passwordHasher.Hash(password1);
        var verified = passwordHasher.Verify(password2,hash);
        if (password1 == password2)
            verified.Should().BeTrue();
        else
            verified.Should().BeFalse();
    }
}