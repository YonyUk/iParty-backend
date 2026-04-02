using FluentValidation.TestHelper;
using NSubstitute;
using Users.Application.Commands;
using Users.Application.Commands.Validators;
using Users.Domain.Rules;

namespace Tests.Unit.Users.Application.Commands.Validators;

public class ChangePasswordCommandValidatorTests
{
    private readonly IUserDomainRulesConfigProvider userDomainRulesConfigProvider;
    private readonly ChangePasswordCommandValidator validator;
    private readonly PasswordDomainRules passwordDomainRules = new PasswordDomainRules(6,10);
    public ChangePasswordCommandValidatorTests()
    {
        userDomainRulesConfigProvider = Substitute.For<IUserDomainRulesConfigProvider>();
        userDomainRulesConfigProvider.PasswordDomainRules.Returns(passwordDomainRules);
        validator = new ChangePasswordCommandValidator(userDomainRulesConfigProvider);
    }

    [Theory]
    [InlineData("yony01uk",true)]
    [InlineData("yony",false)]
    [InlineData("    ",false)]
    [InlineData(null,false)]
    public async Task TestChangePasswordCommandValidator(string? password,bool valid)
    {
        var command = new ChangePasswordCommand(Guid.NewGuid(),password);

        var result = validator.TestValidate(command);
        if (valid)
            result.ShouldNotHaveAnyValidationErrors();
        else
            result.ShouldHaveValidationErrorFor(cmd => cmd.password);
    }
}