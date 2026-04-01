using Common.Domain;
using Users.Domain.Events;
using Users.Domain.ValueObjects;

namespace Users.Domain.Aggregates;

public class User : AggregateRoot<Guid>
{
    public UserName UserName { get; private set; }
    public Email Email { get; private set; }
    public HashedPassword HashedPassword { get; private set; }
    public UserRole Role { get; private set; }
    public User(UserName username, Email email, HashedPassword hashedPassword, UserRole role = UserRole.User)
    : base(Guid.NewGuid())
    {
        if (username == null)
            throw new ArgumentNullException("username","username param can't be null");
        if (email == null)
            throw new ArgumentNullException("email","email param can't be null");
        if (hashedPassword == null)
            throw new ArgumentNullException("hashedPassword","hashedPassword param can't be null");
        UserName = username;
        Email = email;
        HashedPassword = hashedPassword;
        Role = role;
        AddEvent(new UserRegisteredEvent(Id, username, email, role));
    }
    /// <summary>
    /// For testing
    /// </summary>
    /// <param name="id"></param>
    /// <param name="username"></param>
    /// <param name="email"></param>
    /// <param name="hashedPassword"></param>
    /// <param name="role"></param>
    internal User(Guid id, UserName username, Email email, HashedPassword hashedPassword, UserRole role = UserRole.User)
    : base(id)
    {
        UserName = username;
        Email = email;
        HashedPassword = hashedPassword;
        Role = role;
    }
    private User() { }
    public void ChangePassword(HashedPassword hashedPassword)
    {
        HashedPassword = hashedPassword;
        AddEvent(new UserPasswordChangedEvent(Id, hashedPassword));
    }
}