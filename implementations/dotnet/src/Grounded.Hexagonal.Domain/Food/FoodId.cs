namespace Grounded.Hexagonal.Domain.Food;

/// <summary>
/// Identifies one food instance whose spoilage state can evolve over time.
/// </summary>
/// <remarks>
/// This is a domain Value Object.
/// </remarks>
public sealed record FoodId
{
    private FoodId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static FoodId From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(
                "A food identifier cannot be empty.",
                nameof(value));
        }

        return new FoodId(value);
    }

    public static FoodId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
