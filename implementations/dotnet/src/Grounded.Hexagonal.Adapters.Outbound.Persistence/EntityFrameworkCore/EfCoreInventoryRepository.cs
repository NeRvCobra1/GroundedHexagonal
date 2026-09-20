using Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore.Mapping;
using Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore.Models;
using Grounded.Hexagonal.Application.Ports.Outbound;
using Grounded.Hexagonal.Domain.Inventory;
using Microsoft.EntityFrameworkCore;
using InventoryAggregate = Grounded.Hexagonal.Domain.Inventory.Inventory;

namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore;

/// <summary>
/// SQLite/EF Core implementation of PORT-OUT-INVENTORY-001.
/// </summary>
public sealed class EfCoreInventoryRepository : IInventoryRepository
{
    private readonly GroundedDbContextFactory _dbContextFactory;

    public EfCoreInventoryRepository(
        SqlitePersistenceOptions persistenceOptions)
    {
        _dbContextFactory = new GroundedDbContextFactory(
            persistenceOptions);
    }

    public async Task<InventoryAggregate?> GetByPlayerIdAsync(
        PlayerId playerId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(playerId);

        await using var dbContext =
            _dbContextFactory.CreateDbContext();

        var record = await dbContext.Inventories
            .AsNoTracking()
            .Include(inventory => inventory.Items)
            .SingleOrDefaultAsync(
                inventory => inventory.PlayerId == playerId.Value,
                cancellationToken);

        return record is null
            ? null
            : DomainPersistenceMapper.ToDomain(record);
    }

    public async Task SaveAsync(
        InventoryAggregate inventory,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(inventory);

        await using var dbContext =
            _dbContextFactory.CreateDbContext();

        var record = await dbContext.Inventories
            .Include(existing => existing.Items)
            .SingleOrDefaultAsync(
                existing => existing.PlayerId == inventory.PlayerId.Value,
                cancellationToken);

        if (record is null)
        {
            record = new InventoryRecord
            {
                PlayerId = inventory.PlayerId.Value
            };

            dbContext.Inventories.Add(record);
        }

        var desiredItems = inventory
            .GetItems()
            .ToDictionary(
                item => item.ItemId.Value,
                item => item.Quantity);

        foreach (var existingItem in record.Items.ToArray())
        {
            if (desiredItems.Remove(
                existingItem.ItemId,
                out var quantity))
            {
                existingItem.Quantity = quantity;
                continue;
            }

            dbContext.InventoryItems.Remove(existingItem);
        }

        foreach (var desiredItem in desiredItems)
        {
            record.Items.Add(
                new InventoryItemRecord
                {
                    PlayerId = inventory.PlayerId.Value,
                    ItemId = desiredItem.Key,
                    Quantity = desiredItem.Value
                });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
