using FluentValidation.TestHelper;
using NSubstitute;
using Users.Application.Commands;
using Users.Application.Commands.Validators;
using Users.Application.DTOs;
using Users.Domain.Rules;

namespace Tests.Unit.Users.Application.Commands.Validators;

public class LoginUserCommandValidatorTests
{
    private readonly IUserDomainRulesConfigProvider userDomainRulesConfigProvider;
    private readonly LoginUserCommandValidator validator;
    private readonly UserNameDomainRules userNameDomainRules = new UserNameDomainRules(6,10);
    private readonly PasswordDomainRules passwordDomainRules = new PasswordDomainRules(6,10);
    public LoginUserCommandValidatorTests()
    {
        userDomainRulesConfigProvider = Substitute.For<IUserDomainRulesConfigProvider>();
        userDomainRulesConfigProvider.UserNameDomainRules.Returns(userNameDomainRules);
        userDomainRulesConfigProvider.PasswordDomainRules.Returns(passwordDomainRules);
        validator = new LoginUserCommandValidator(userDomainRulesConfigProvider);
    }

    [Theory]
    [InlineData("yonyuk","yony01uk",true,true)]
    [InlineData("yonyuk","yony",true,false)]
    [InlineData("yonyuk","    ",true,false)]
    [InlineData("yonyuk",null,true,false)]
    [InlineData("yony","yony01uk",false,true)]
    [InlineData("    ","yony01uk",false,true)]
    [InlineData(null,"yony01uk",false,true)]
    [InlineData(null,null,false,false)]
    [InlineData(null,"    ",false,false)]
    [InlineData(null,"yony",false,false)]
    [InlineData("yony",null,false,false)]
    [InlineData("    ",null,false,false)]
    [InlineData("    ","    ",false,false)]
    public async Task TestLoginUserCommandValidator(
        string? username,
        string? password,
        bool usernameValid,
        bool passwordValid
    )
    {
        var dto = new LoginUserDTO(username,password);
        var command = new LoginUserCommand(dto);
        
        var result = validator.TestValidate(command);

        if (usernameValid && passwordValid)
            result.ShouldNotHaveAnyValidationErrors();
        else
        {
            if (!usernameValid)
                result.ShouldHaveValidationErrorFor(cmd => cmd.data.UserName);
            if (!passwordValid)
                result.ShouldHaveValidationErrorFor(cmd => cmd.data.Password);
        }
    }
}