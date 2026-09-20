using Grounded.Hexagonal.Application.Ports.Inbound;
using Grounded.Hexagonal.Application.Ports.Outbound;

namespace Grounded.Hexagonal.Application.UseCases.GetInventory;

/// <summary>
/// Coordinates UC-INVENTORY-001.
/// </summary>
/// <remarks>
/// Architecture ID: UC-INVENTORY-001
///
/// This is a query use case. It loads the Inventory through the existing
/// outbound port and maps the domain snapshot to an Application result.
/// It does not persist changes.
/// </remarks>
public sealed class GetInventoryHandler : IGetInventoryUseCase
{
    private readonly IInventoryRepository _inventoryRepository;

    public GetInventoryHandler(
        IInventoryRepository inventoryRepository)
    {
        ArgumentNullException.ThrowIfNull(inventoryRepository);
        _inventoryRepository = inventoryRepository;
    }

    public async Task<GetInventoryResult> ExecuteAsync(
        GetInventoryQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var inventory =
            await _inventoryRepository.GetByPlayerIdAsync(
                query.PlayerId,
                cancellationToken);

        if (inventory is null)
        {
            return GetInventoryResult.InventoryNotFound();
        }

        var items = inventory
            .GetItems()
            .Select(item => new InventoryItemResult(
                item.ItemId,
                item.Quantity));

        return GetInventoryResult.Success(items);
    }
}
