using Users.Domain.Exceptions;
using Users.Domain.Rules;

namespace Users.Domain.ValueObjects;

public record UserName
{
    public string Value { get; private set; }
    public UserName(string value, UserNameDomainRules rules)
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            throw new InvalidUserNameException("value must be not null nor whitespace");
        rules.Check(value);
        Value = value;
    }
    public UserName() { }
}