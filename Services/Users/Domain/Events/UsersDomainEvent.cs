using Common.Domain;

namespace Users.Domain.Events;

public record UsersDomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; protected set; }
    protected UsersDomainEvent(DateTime OccurredOn) => this.OccurredOn = OccurredOn;
}