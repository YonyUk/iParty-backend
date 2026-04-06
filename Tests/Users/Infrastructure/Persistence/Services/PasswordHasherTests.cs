using Users.Infrastructure.Services.Security;

namespace Tests.Unit.Users.Infrastructure.Persistence.Services;

public class PasswordHasherTests
{
    [Fact]
    public void TestName()
    {
        var hasher = new PasswordHasher();
        string hash = hasher.Hash("password");
        Assert.NotEmpty(hash);
    }
    
}