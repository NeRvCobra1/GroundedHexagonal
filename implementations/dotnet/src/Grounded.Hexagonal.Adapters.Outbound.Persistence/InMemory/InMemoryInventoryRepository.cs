using System.Collections.Concurrent;
using Grounded.Hexagonal.Application.Ports.Outbound;
using Grounded.Hexagonal.Domain.Inventory;
using InventoryAggregate = Grounded.Hexagonal.Domain.Inventory.Inventory;

namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.InMemory;

/// <summary>
/// In-memory implementation of PORT-OUT-INVENTORY-001.
/// </summary>
/// <remarks>
/// This class is an outbound adapter.
///
/// The Application contract is <see cref="IInventoryRepository"/>.
/// The fact that this implementation stores objects in memory is an
/// infrastructure decision and is intentionally invisible to Application.
/// </remarks>
public sealed class InMemoryInventoryRepository : IInventoryRepository
{
    private readonly ConcurrentDictionary<PlayerId, InventoryAggregate> _inventories;

    public InMemoryInventoryRepository()
        : this(Array.Empty<InventoryAggregate>())
    {
    }

    public InMemoryInventoryRepository(IEnumerable<InventoryAggregate> inventories)
    {
        ArgumentNullException.ThrowIfNull(inventories);

        _inventories = new ConcurrentDictionary<PlayerId, InventoryAggregate>(
            inventories.ToDictionary(
                inventory => inventory.PlayerId,
                inventory => inventory));
    }

    public Task<InventoryAggregate?> GetByPlayerIdAsync(
        PlayerId playerId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(playerId);
        cancellationToken.ThrowIfCancellationRequested();

        _inventories.TryGetValue(playerId, out var inventory);

        return Task.FromResult(inventory);
    }

    public Task SaveAsync(
        InventoryAggregate inventory,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(inventory);
        cancellationToken.ThrowIfCancellationRequested();

        _inventories[inventory.PlayerId] = inventory;

        return Task.CompletedTask;
    }
}
