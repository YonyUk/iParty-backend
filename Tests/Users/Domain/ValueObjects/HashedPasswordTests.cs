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

    [Theory]
    [InlineData("aksdsgffdhsf","aksdsgffdhsf",true)]
    [InlineData("aksdsgffdhsf","aksdsgffdhsf1",false)]
    public void TestHashedPasswordEquality(string hash1, string hash2, bool equals)
    {
        var hashedpasswordObject1 = new HashedPassword(hash1);
        var hashedpasswordObject2 = new HashedPassword(hash2);
        (hashedpasswordObject1 == hashedpasswordObject2).Should().Be(equals ? true : false);
    }
}