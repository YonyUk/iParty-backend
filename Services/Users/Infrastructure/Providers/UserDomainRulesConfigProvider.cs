using Microsoft.Extensions.Options;
using Users.Domain.Rules;

namespace Users.Infrastructure.Providers;

public record UserDomainRulesConfigProvider : IUserDomainRulesConfigProvider
{
    public UserNameDomainRules UserNameDomainRules { get; private set; }

    public PasswordDomainRules PasswordDomainRules { get; private set; }
    public UserDomainRulesConfigProvider(IOptions<UserDomainRulesOptions> options)
    {
        var rules = options.Value;
        UserNameDomainRules = new UserNameDomainRules(rules.MinimumUserNameLength,rules.MaximumUserNameLength);
        PasswordDomainRules = new PasswordDomainRules(rules.MinimumPasswordLength,rules.MaximumPasswordLength);
    }
}