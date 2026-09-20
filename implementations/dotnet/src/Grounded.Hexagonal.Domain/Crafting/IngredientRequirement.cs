using Grounded.Hexagonal.Domain.Items;

namespace Grounded.Hexagonal.Domain.Crafting;

/// <summary>
/// Represents the amount of one item required by a recipe.
/// </summary>
/// <remarks>
/// This is a domain Value Object.
/// </remarks>
public sealed record IngredientRequirement
{
    public IngredientRequirement(ItemId itemId, int quantity)
    {
        ArgumentNullException.ThrowIfNull(itemId);

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                quantity,
                "An ingredient requirement must be greater than zero.");
        }

        ItemId = itemId;
        Quantity = quantity;
    }

    public ItemId ItemId { get; }

    public int Quantity { get; }
}
