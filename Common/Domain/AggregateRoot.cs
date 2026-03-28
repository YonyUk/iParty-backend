namespace Common.Domain;

public abstract class AggregateRoot<TId> : Entity<TId>
{
    private readonly List<IDomainEvent> events = new();
    public IReadOnlyList<IDomainEvent> Events => events.AsReadOnly();
    protected AggregateRoot(TId id) : base(id) { }
    protected AggregateRoot() { }
    protected void AddEvent(IDomainEvent _event) => events.Add(_event);
    public void ClearEvents() => events.Clear();
}