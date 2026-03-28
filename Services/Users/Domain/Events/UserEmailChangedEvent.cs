using Users.Domain.ValueObjects;

namespace Users.Domain.Events;

public record UserEmailChangedEvent : UsersDomainEvent
{
    public Guid Id {get;private set;}
    public Email Email{get; private set;}
    public UserEmailChangedEvent(Guid id,Email email) : base(DateTime.UtcNow)
    {
        Id = id;
        Email = email;
    }
    internal UserEmailChangedEvent(Guid id,Email email,DateTime occurredOn)
    : this(id, email)
    {
        OccurredOn = occurredOn;
    }
}