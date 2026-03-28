using Users.Domain.Exceptions;

namespace Users.Domain.ValueObjects;
public record HashedPassword
{
    public string Value { get; private set;}
    public HashedPassword(string value)
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            throw new InvalidHashedPasswordException("value must be not null nor whitespace");
        Value = value;
    }
}