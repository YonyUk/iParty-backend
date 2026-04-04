using FluentValidation.TestHelper;
using NSubstitute;
using Users.Application.Querys;
using Users.Application.Querys.Validators;
using Users.Domain.Rules;

namespace Tests.Unit.Users.Application.Queries.Validators;

public class GetUserByUserNameQueryValidatorTests
{
    private readonly GetUserByUserNameCommandValidator validator;
    private readonly IUserDomainRulesConfigProvider provider;
    private readonly UserNameDomainRules rules = new UserNameDomainRules(6,10);
    public GetUserByUserNameQueryValidatorTests()
    {
        provider = Substitute.For<IUserDomainRulesConfigProvider>();
        provider.UserNameDomainRules.Returns(rules);
        validator = new GetUserByUserNameCommandValidator(provider);
    }

    [Theory]
    [InlineData("yonyuk",true)]
    [InlineData("yony",false)]
    [InlineData("     ",false)]
    [InlineData(null,false)]
    public async Task TestGetUserByUserNameQueryValidator(string? username,bool valid)
    {
        var query = new GetUserByUserNameQuery(username);

        var result = validator.TestValidate(query);

        if (valid)
            result.ShouldNotHaveAnyValidationErrors();
        else
            result.ShouldHaveValidationErrorFor(q => q.UserName);
    }
}