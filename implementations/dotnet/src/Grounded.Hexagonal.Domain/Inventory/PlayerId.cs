namespace Grounded.Hexagonal.Domain.Inventory;

/// <summary>
/// Identifies the player that owns an inventory.
/// </summary>
/// <remarks>
/// This is a domain Value Object.
/// The identifier has no knowledge of authentication, HTTP or persistence.
/// </remarks>
public sealed record PlayerId
{
    private PlayerId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static PlayerId From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(
                "A player identifier cannot be empty.",
                nameof(value));
        }

        return new PlayerId(value);
    }

    public static PlayerId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
