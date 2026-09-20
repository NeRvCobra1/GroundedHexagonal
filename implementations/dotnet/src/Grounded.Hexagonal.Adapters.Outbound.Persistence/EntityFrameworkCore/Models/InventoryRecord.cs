namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore.Models;

internal sealed class InventoryRecord
{
    public Guid PlayerId { get; set; }

    public List<InventoryItemRecord> Items { get; set; } = [];
}
