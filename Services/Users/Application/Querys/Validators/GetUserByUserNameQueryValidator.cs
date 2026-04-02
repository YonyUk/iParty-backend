using FluentValidation;
using Users.Domain.Rules;

namespace Users.Application.Querys.Validators;

public class GetUserByUserNameCommandValidator : AbstractValidator<GetUserByUserNameQuery>
{
    public GetUserByUserNameCommandValidator(IUserDomainRulesConfigProvider provider)
    {
        var rules = provider.UserNameDomainRules;
        RuleFor(command => command.UserName)
            .NotEmpty().WithMessage("UserName field is required")
            .MinimumLength(rules.MinLength)
            .WithMessage($"UserName must have at least ${rules.MinLength} characters")
            .MaximumLength(rules.MaxLength)
            .WithMessage($"UserName can't exceed ${rules.MaxLength} characters");
    }
}