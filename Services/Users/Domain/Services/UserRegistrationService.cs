using Users.Domain.Aggregates;
using Users.Domain.Exceptions;
using Users.Domain.ValueObjects;

namespace Users.Domain.Services;

public class UserRegistrationService
{
    private readonly IUserUniquenessChecker checker;
    public UserRegistrationService(IUserUniquenessChecker checker) => this.checker = checker;
    public async Task<User> RegisterUser(UserName username,Email email,HashedPassword hashedPassword,UserRole role = UserRole.User)
    {
        if (!await checker.IsUnique(username))
            throw new UserAlreadyExistsException(nameof(username), username.Value);
        if (!await checker.IsUnique(email))
            throw new UserAlreadyExistsException(nameof(email), email.Value);
        return new User(username, email, hashedPassword, role);
    }
}