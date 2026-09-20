namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore.Models;

internal sealed class InventoryItemRecord
{
    public Guid PlayerId { get; set; }

    public Guid ItemId { get; set; }

    public int Quantity { get; set; }

    public InventoryRecord Inventory { get; set; } = null!;
}
