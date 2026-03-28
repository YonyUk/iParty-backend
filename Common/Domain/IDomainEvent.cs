namespace Common.Domain;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}