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
<<<<<<< HEAD
    [InlineData(null,false)]
    [InlineData("      ",false)]
    public void TestCreateUserName(string? username,bool valid)
=======
    public void TestCreateUser(string username,bool valid)
>>>>>>> 2bc2bc6 (adds User creation test)
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
<<<<<<< HEAD
    [Theory]
    [InlineData("yonyuk","yonyuk",true)]
    [InlineData("yonyuk","yony01uk",false)]
    public void TestUserNameEquality(string username1,string username2,bool equals)
    {
        var usernameObject1 = new UserName(username1,rules);
        var usernameObject2 = new UserName(username2,rules);
        (usernameObject1 == usernameObject2).Should().Be(equals ? true : false);
    }
=======
>>>>>>> 2bc2bc6 (adds User creation test)
}