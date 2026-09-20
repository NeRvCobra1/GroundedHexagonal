using Grounded.Hexagonal.Domain.Items;

namespace Grounded.Hexagonal.Domain.Inventory;

/// <summary>
/// Read-only domain value representing the quantity of one item in an inventory.
/// </summary>
/// <remarks>
/// This Value Object is used to expose inventory contents without exposing the
/// mutable dictionary used internally by the Inventory entity.
/// </remarks>
public sealed record InventoryItemQuantity
{
    public InventoryItemQuantity(
        ItemId itemId,
        int quantity)
    {
        ArgumentNullException.ThrowIfNull(itemId);

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                quantity,
                "An inventory item quantity must be greater than zero.");
        }

        ItemId = itemId;
        Quantity = quantity;
    }

    public ItemId ItemId { get; }

    public int Quantity { get; }
}
