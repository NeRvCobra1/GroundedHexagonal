namespace Grounded.Hexagonal.Domain.Crafting;

/// <summary>
/// Identifies a crafting recipe.
/// </summary>
/// <remarks>
/// This is a domain Value Object.
/// </remarks>
public sealed record RecipeId
{
    private RecipeId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static RecipeId From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(
                "A recipe identifier cannot be empty.",
                nameof(value));
        }

        return new RecipeId(value);
    }

    public static RecipeId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
