namespace Commerce.SharedKernel.Domain;

public abstract class Entity
{
    public Guid Id { get; }

    // Protected constructor for EF Core or other ORMs
    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    // Override equality operators
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;

        return ReferenceEquals(this, other) || Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    // Equality operator overloads
    public static bool operator ==(Entity? a, Entity? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity? a, Entity? b)
    {
        return !(a == b);
    }
}