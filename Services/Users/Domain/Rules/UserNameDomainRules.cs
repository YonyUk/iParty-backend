using System.Text.RegularExpressions;
using Users.Domain.Exceptions;

namespace Users.Domain.Rules;

public record UserNameDomainRules(int MinLength,int MaxLength)
{
    Regex pattern = new Regex(@"\w{" + $"{MinLength},{MaxLength}" + "}");
    public void Check(string value)
    {
        var match = pattern.Match(value);
        if (!match.Success) throw new InvalidUserNameException("Invalid format for username");
    }
}