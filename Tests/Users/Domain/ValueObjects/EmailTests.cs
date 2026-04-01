using FluentAssertions;
using Users.Domain.Exceptions;
using Users.Domain.ValueObjects;

namespace Tests.Unit.Users.Domain.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("user@gmail.com",true)]
    [InlineData("usergmail.com",false)]
    [InlineData(null,false)]
    [InlineData("        ",false)]
    public void TestCreateEmail(string? email,bool valid)
    {
        var action = () =>
        {
            var emailObject = new Email(email);
            if (valid)
                emailObject.Value.Should().Be(email);
        };
        if (valid)
            action.Should().NotThrow();
        else
            action.Should().Throw<InvalidEmailException>();
    }
}