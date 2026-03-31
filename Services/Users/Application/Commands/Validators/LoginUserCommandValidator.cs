using FluentValidation;
using Users.Domain.Rules;

namespace Users.Application.Commands.Validators;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator(IUserDomainRulesConfigProvider provider)
    {
        var usernameRules = provider.UserNameDomainRules;
        var passwordRules = provider.PasswordDomainRules;

        RuleFor(command => command.data.UserName)
            .NotEmpty().WithMessage("UserName field is required")
            .MaximumLength(usernameRules.MaxLength)
            .WithMessage($"Username can't exceed {usernameRules.MaxLength} characters")
            .MinimumLength(usernameRules.MinLength)
            .WithMessage($"UserName must hav at least {usernameRules.MinLength} characters");

        RuleFor(command => command.data.Password)
            .NotEmpty().WithMessage("Password field is required")
            .MaximumLength(passwordRules.MaxLength)
            .WithMessage($"Password can't exceed {passwordRules.MaxLength} characters")
            .MinimumLength(passwordRules.MinLength)
            .WithMessage($"Password must hav at least {passwordRules.MinLength} characters");

    }
}