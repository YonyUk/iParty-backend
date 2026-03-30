using FluentValidation;

namespace Users.Application.Commands.Validators;

public class GetUserByEmailCommandValidator : AbstractValidator<GetUserByEmailCommand>
{
    public GetUserByEmailCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty().WithMessage("Email field is required")
            .EmailAddress().WithMessage("Invalid email format");
    }
}