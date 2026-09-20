using Grounded.Hexagonal.Domain.Inventory;
using InventoryAggregate = Grounded.Hexagonal.Domain.Inventory.Inventory;

namespace Grounded.Hexagonal.Application.Ports.Outbound;

/// <summary>
/// Outbound port used by Application to load and persist inventories.
/// </summary>
/// <remarks>
/// Architecture ID: PORT-OUT-INVENTORY-001
///
/// Application owns this contract. A concrete outbound adapter will decide
/// how inventories are actually stored.
/// </remarks>
public interface IInventoryRepository
{
    Task<InventoryAggregate?> GetByPlayerIdAsync(
        PlayerId playerId,
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        InventoryAggregate inventory,
        CancellationToken cancellationToken = default);
}
