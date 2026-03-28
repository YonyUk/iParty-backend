namespace Common.Domain;

public abstract class Entity<TId> : IEquatable<Entity<TId>>
{
    public TId? Id { get; protected set; }
    protected Entity(TId id) => Id = id;
    protected Entity() { }
    public bool Equals(Entity<TId>? other) => other != null ? Equals(Id,other.Id) : false;
    public override bool Equals(object? obj) => obj is Entity<TId> other && Equals(Id,other.Id);
    public override int GetHashCode() => Id != null ? Id.GetHashCode() : 0;
}