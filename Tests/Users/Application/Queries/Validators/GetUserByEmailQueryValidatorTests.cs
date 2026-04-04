using FluentValidation.TestHelper;
using Users.Application.Querys;
using Users.Application.Querys.Validators;
using Users.Domain.ValueObjects;

namespace Tests.Unit.Users.Application.Queries.Validators;

public class GetUserByEmailQueryValidatorTests
{
    private readonly GetUserByEmailQueryValidator validator;
    public GetUserByEmailQueryValidatorTests()
    {
        validator = new GetUserByEmailQueryValidator();
    }

    [Theory]
    [InlineData("user@gmail.com",true)]
    [InlineData("usergmail.com",false)]
    [InlineData(null,false)]
    [InlineData("   ",false)]
    public async Task TestGetUserByEmailQueryValidator(string? email,bool valid)
    {
        var query = new GetUserByEmailQuery(email);

        var result = validator.TestValidate(query);

        if (valid)
            result.ShouldNotHaveAnyValidationErrors();
        else
            result.ShouldHaveValidationErrorFor(q => q.Email);    
    }
}