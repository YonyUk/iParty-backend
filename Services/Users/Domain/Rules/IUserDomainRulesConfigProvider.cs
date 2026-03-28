namespace Users.Domain.Rules;

public interface IUserDomainRulesConfigProvider
{
    UserNameDomainRules UserNameDomainRules { get; }
    PasswordDomainRules PasswordDomainRules { get; }
}