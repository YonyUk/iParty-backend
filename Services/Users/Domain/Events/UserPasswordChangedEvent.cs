using Users.Domain.ValueObjects;

namespace Users.Domain.Events;

public record UserPasswordChangedEvent : UsersDomainEvent
{
    public Guid Id { get; private set; }
    public HashedPassword HashedPassword { get; private set; }
    public UserPasswordChangedEvent(Guid id,HashedPassword hashedPassword)
    :base(DateTime.UtcNow)
    {
        Id = id;
        HashedPassword = hashedPassword;
    }
    internal UserPasswordChangedEvent(Guid id,HashedPassword hashedPassword,DateTime occurredOn)
    : this(id, hashedPassword)
    {
        OccurredOn = occurredOn;
    }
}