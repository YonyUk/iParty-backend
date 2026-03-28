using Users.Domain.ValueObjects;

namespace Users.Domain.Events;

public record UserRegisteredEvent : UsersDomainEvent
{
    public Guid Id { get; private set; }
    public UserName UserName { get; private set; }
    public Email Email { get; private set;}
    public UserRole Role { get; private set;}
    public UserRegisteredEvent(Guid id,UserName username,Email email,UserRole role = UserRole.User)
    : base(DateTime.UtcNow)
    {
        Id = id;
        UserName = username;
        Email = email;
        Role = role;   
    }
    /// <summary>
    /// For testing
    /// </summary>
    /// <param name="id"></param>
    /// <param name="username"></param>
    /// <param name="email"></param>
    /// <param name="occurredOn"></param>
    /// <param name="role"></param>
    internal UserRegisteredEvent(Guid id,UserName username,Email email,DateTime occurredOn, UserRole role = UserRole.User)
    : this(id,username,email,role)
    {
        OccurredOn = occurredOn;
    }
}