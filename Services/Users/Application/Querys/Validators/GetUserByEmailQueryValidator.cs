using FluentValidation;

namespace Users.Application.Querys.Validators;

public class GetUserByEmailQueryValidator : AbstractValidator<GetUserByEmailQuery>
{
    public GetUserByEmailQueryValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty().WithMessage("Email field is required")
            .EmailAddress().WithMessage("Invalid email format");
    }
}