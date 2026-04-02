using FluentValidation.TestHelper;
using NSubstitute;
using Users.Application.Commands;
using Users.Application.Commands.Validators;
using Users.Application.DTOs;
using Users.Domain;
using Users.Domain.Rules;

namespace Tests.Unit.Users.Application.Commands.Validators;

public class RegisterUserCommandValidatorTests
{
    private readonly IUserDomainRulesConfigProvider userDomainRulesConfigProvider;
    private readonly UserNameDomainRules userNameDomainRules = new UserNameDomainRules(6, 10);
    private readonly PasswordDomainRules passwordDomainRules = new PasswordDomainRules(6, 10);
    private readonly RegisterUserCommandValidator validator;
    public RegisterUserCommandValidatorTests()
    {
        userDomainRulesConfigProvider = Substitute.For<IUserDomainRulesConfigProvider>();
        userDomainRulesConfigProvider.UserNameDomainRules.Returns(userNameDomainRules);
        userDomainRulesConfigProvider.PasswordDomainRules.Returns(passwordDomainRules);
        validator = new RegisterUserCommandValidator(userDomainRulesConfigProvider);
    }

    [Theory]
    [InlineData("yonyuk","user@gmail.com","yony01uk",true,true,true)]
    [InlineData("yony","user@gmail.com","yony01uk",false,true,true)]
    [InlineData(null,"user@gmail.com","yony01uk",false,true,true)]
    [InlineData("    ","user@gmail.com","yony01uk",false,true,true)]
    [InlineData("yonyuk","usergmail.com","yony01uk",true,false,true)]
    [InlineData("yonyuk",null,"yony01uk",true,false,true)]
    [InlineData("yonyuk","      ","yony01uk",true,false,true)]
    [InlineData("yonyuk","user@gmail.com","yony",true,true,false)]
    [InlineData("yonyuk","user@gmail.com",null,true,true,false)]
    [InlineData("yonyuk","user@gmail.com","    ",true,true,false)]
    [InlineData("yony","user@gmail.com","yony",false,true,false)]
    [InlineData(null,"user@gmail.com","yony",false,true,false)]
    [InlineData("    ","user@gmail.com","yony",false,true,false)]
    [InlineData("yony","user@gmail.com",null,false,true,false)]
    [InlineData("yony","user@gmail.com","    ",false,true,false)]
    [InlineData(null,"user@gmail.com",null,false,true,false)]
    [InlineData("   ","user@gmail.com","    ",false,true,false)]
    [InlineData("yonyuk","usergmail.com","yony",true,false,false)]
    [InlineData("yonyuk","usergmail.com",null,true,false,false)]
    [InlineData("yonyuk","usergmail.com","   ",true,false,false)]
    [InlineData("yonyuk",null,"yony",true,false,false)]
    [InlineData("yonyuk","    ","yony",true,false,false)]
    [InlineData("yonyuk",null,null,true,false,false)]
    [InlineData("yonyuk","    ","    ",true,false,false)]
    [InlineData("yony","usergmail.com","yonyuk",false,false,true)]
    [InlineData("yony",null,"yonyuk",false,false,true)]
    [InlineData("yony","    ","yonyuk",false,false,true)]
    [InlineData(null,"usergmail.com","yonyuk",false,false,true)]
    [InlineData("    ","usergmail.com","yonyuk",false,false,true)]
    [InlineData(null,null,"yonyuk",false,false,true)]
    [InlineData("    ","    ","yonyuk",false,false,true)]
    [InlineData("yony","usergmail.com","yony",false,false,false)]
    [InlineData("yony","usergmail.com",null,false,false,false)]
    [InlineData("yony",null,"yony",false,false,false)]
    [InlineData(null,"usergmail.com","yony",false,false,false)]
    [InlineData("yony","usergmail.com","    ",false,false,false)]
    [InlineData("yony","    ","yony",false,false,false)]
    [InlineData("    ","usergmail.com","yony",false,false,false)]
    [InlineData("yony",null,null,false,false,false)]
    [InlineData("    ",null,null,false,false,false)]
    [InlineData(null,"usergmail.com",null,false,false,false)]
    [InlineData(null,"   ",null,false,false,false)]
    [InlineData("yony","    ",null,false,false,false)]
    [InlineData("    ","usergmail.com",null,false,false,false)]
    [InlineData(null,null,"yony",false,false,false)]
    [InlineData(null,null,"    ",false,false,false)]
    public async Task TestRegisterUserCommandValidator(
        string? username,
        string? email,
        string? password,
        bool usernameValid,
        bool emailValid,
        bool passwordValid
    )
    {
        var dto = new RegisterUserDTO(username, email, password, UserRole.User);
        var command = new RegisterUserCommand(dto);

        var result = validator.TestValidate(command);

        if (usernameValid && emailValid && passwordValid)
            result.ShouldNotHaveAnyValidationErrors();
        else
        {
            if (!usernameValid)
                result.ShouldHaveValidationErrorFor(cmd => cmd.data.UserName);
            if (!emailValid)
                result.ShouldHaveValidationErrorFor(cmd => cmd.data.Email);
            if (!passwordValid)
                result.ShouldHaveValidationErrorFor(cmd => cmd.data.Password);
        }
    }
}