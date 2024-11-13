using UuidExtensions;

namespace Commerce.SharedKernel.Domain.ValueObjects.Identifiers;

public abstract class EntityId<T> where T : new()
{
    protected string Value { get; }

    protected abstract string Prefix { get; init; }

    protected EntityId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("ID value cannot be null or whitespace.", nameof(value));

        if (!value.StartsWith($"{Prefix}:"))
            throw new ArgumentException($"Invalid identifier. Must start with {Prefix}:", nameof(value));

        Value = value;
    }

    protected EntityId()
    {
        Value = $"{Prefix}:{Uuid7.Id25()}";
    }

    public override bool Equals(object? obj)
        => Equals(obj as EntityId<T>);

    public bool Equals(EntityId<T>? other)
        => other is not null && Value == other.Value;

    public override int GetHashCode() => HashCode.Combine(Value);

    public static bool operator ==(EntityId<T> left, EntityId<T> right)
        => Equals(left, right);

    public static bool operator !=(EntityId<T> left, EntityId<T> right)
        => !Equals(left, right);
    public override string ToString() => Value;
    public static T NewId() => new();

}