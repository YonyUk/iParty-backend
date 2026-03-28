using System.Text.RegularExpressions;
using Users.Domain.Exceptions;

namespace Users.Domain.Rules;

public record PasswordDomainRules(int MinLength,int MaxLength)
{
    Regex pattern = new Regex(@"^[^\s]{" + $"{MinLength},{MaxLength}" + "}$");
    public void Check(string value)
    {
        var match = pattern.Match(value);
        if (!match.Success) throw new InvalidPasswordException("Password size not valid");
    }
}