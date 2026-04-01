using FluentAssertions;
using Users.Domain.Exceptions;
using Users.Domain.Rules;
using Users.Domain.ValueObjects;

namespace Tests.Unit.Users.Domain.ValueObjects;

public class UserNameTests
{
    private readonly UserNameDomainRules rules = new UserNameDomainRules(6,10);

    [Theory]
    [InlineData("yonyuk",true)]
    [InlineData("yony",false)]
    [InlineData(null,false)]
    [InlineData("      ",false)]
    public void TestCreateUserName(string? username,bool valid)
    {
        var action = () =>
        {
            var usernameObject = new UserName(username,rules);
            if (valid)
                usernameObject.Value.Should().Be(username);
        };
        if (valid)
            action.Should().NotThrow();
        else
            action.Should().Throw<InvalidUserNameException>();
    }
}