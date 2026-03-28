using Common.Domain;
using MediatR;

namespace Users.Infrastructure.Persistence;

public class DomainEventNotification<TEvent> : INotification where TEvent : IDomainEvent
{
    public TEvent DomainEvent { get; private set;}
    public DomainEventNotification(TEvent domainEvent) => DomainEvent = domainEvent;
}