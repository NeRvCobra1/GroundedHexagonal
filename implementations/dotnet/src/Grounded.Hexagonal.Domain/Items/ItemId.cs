namespace Grounded.Hexagonal.Domain.Items;

/// <summary>
/// Identifies an item type inside the domain.
/// </summary>
/// <remarks>
/// This is a domain Value Object.
/// It is independent from database keys, HTTP values or persistence models.
/// </remarks>
public sealed record ItemId
{
    private ItemId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static ItemId From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(
                "An item identifier cannot be empty.",
                nameof(value));
        }

        return new ItemId(value);
    }

    public static ItemId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
