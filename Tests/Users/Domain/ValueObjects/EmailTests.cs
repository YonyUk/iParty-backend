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

    [Theory]
    [InlineData("user@gmail.com","user@gmail.com",true)]
    [InlineData("user@gmail.com","test@gmail.com",false)]
    public void TestEmailEquality(string email1,string email2,bool equals)
    {
        var emailObject1 = new Email(email1);
        var emailObject2 = new Email(email2);
        (emailObject1 == emailObject2).Should().Be(equals ? true : false);
    }
}