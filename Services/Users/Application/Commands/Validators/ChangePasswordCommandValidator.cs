using FluentValidation;
using Users.Domain.Rules;

namespace Users.Application.Commands.Validators;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    private readonly PasswordDomainRules passwordDomainRules;
    public ChangePasswordCommandValidator(IUserDomainRulesConfigProvider userDomainRulesConfigProvider)
    {
        passwordDomainRules = userDomainRulesConfigProvider.PasswordDomainRules;

        RuleFor(command => command.password)
            .NotEmpty().WithMessage("Password field is required")
            .MinimumLength(passwordDomainRules.MinLength)
            .WithMessage($"Password must have at least {passwordDomainRules.MinLength} characters")
            .MaximumLength(passwordDomainRules.MaxLength)
            .WithMessage($"Password can't exceed {passwordDomainRules.MaxLength} characters");
    }
}