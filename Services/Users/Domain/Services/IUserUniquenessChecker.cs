using Users.Domain.ValueObjects;

namespace Users.Domain.Services;

public interface IUserUniquenessChecker
{
    Task<bool> IsUnique(UserName username);
    Task<bool> IsUnique(Email email);
}