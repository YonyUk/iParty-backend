using FluentAssertions;
using Users.Domain.Exceptions;
using Users.Domain.ValueObjects;

namespace Tests.Unit.Users.Domain.ValueObjects;

public class HashedPasswordTests
{
    [Theory]
    [InlineData("asldsygcauiyg3q",true)]
    [InlineData(null,false)]
    [InlineData("     ",false)]
    public void TestCreateHashedPassword(string? hash,bool valid)
    {
        var action = () =>
        {
            var hashedpasswordObject = new HashedPassword(hash);
            if (valid)
                hashedpasswordObject.Value.Should().Be(hash);
        };
        if (valid)
            action.Should().NotThrow();
        else
            action.Should().Throw<InvalidHashedPasswordException>();
    }
}