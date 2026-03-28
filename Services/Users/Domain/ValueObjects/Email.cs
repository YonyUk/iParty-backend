using System.Net.Mail;
using Users.Domain.Exceptions;

namespace Users.Domain.ValueObjects;

public record Email
{
    public string Value { get; private set; }
    public Email(string value)
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            throw new InvalidEmailException("value must be not null nor whitespace");
        try
        {
            var mail = new MailAddress(value);
            Value = mail.Address;
        }
        catch (FormatException)
        {
            throw new InvalidEmailException("Invalid email format");
        }
    }
}