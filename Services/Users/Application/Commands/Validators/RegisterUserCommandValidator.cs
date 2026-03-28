using FluentValidation;
using Users.Domain.Rules;

namespace Users.Application.Commands.Validators;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator(IUserDomainRulesConfigProvider provider)
    {
        var usernameRules = provider.UserNameDomainRules;
        var passwordRules = provider.PasswordDomainRules;

        RuleFor(command => command.data.UserName)
            .NotEmpty().WithMessage("UserName field is required")
            .MinimumLength(usernameRules.MinLength)
            .WithMessage($"UserName must have at least ${usernameRules.MinLength} characters")
            .MaximumLength(usernameRules.MaxLength)
            .WithMessage($"UserName can't exceed {usernameRules.MaxLength} characters");
        
        RuleFor(command => command.data.Email)
            .NotEmpty().WithMessage("Email field is required")
            .EmailAddress().WithMessage("Invalid email format");
        
        RuleFor(command => command.data.Password)
            .NotEmpty().WithMessage("Password field is required")
            .MinimumLength(passwordRules.MinLength)
            .WithMessage($"Password must have at least ${passwordRules.MinLength} characters")
            .MaximumLength(passwordRules.MaxLength)
            .WithMessage($"Password can't exceed {passwordRules.MaxLength} characters");
    }
}