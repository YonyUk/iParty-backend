namespace Users.Domain.Rules;

public record UserDomainRulesOptions(
    int MinimumUserNameLength,
    int MaximumUserNameLength,
    int MinimumPasswordLength,
    int MaximumPasswordLength
)
{
    public UserDomainRulesOptions() : this(0, 0, 0, 0) { }
}