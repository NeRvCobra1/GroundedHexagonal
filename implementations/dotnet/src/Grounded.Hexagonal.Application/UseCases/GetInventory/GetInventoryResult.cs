using Grounded.Hexagonal.Domain.Items;

namespace Grounded.Hexagonal.Application.UseCases.GetInventory;

public enum GetInventoryStatus
{
    Success,
    InventoryNotFound
}

/// <summary>
/// One item returned by the GetInventory use case.
/// </summary>
public sealed record InventoryItemResult(
    ItemId ItemId,
    int Quantity);

/// <summary>
/// Application-level result for UC-INVENTORY-001.
/// </summary>
public sealed record GetInventoryResult
{
    private GetInventoryResult(
        GetInventoryStatus status,
        IReadOnlyCollection<InventoryItemResult> items)
    {
        Status = status;
        Items = items;
    }

    public GetInventoryStatus Status { get; }

    public IReadOnlyCollection<InventoryItemResult> Items { get; }

    public bool IsSuccess => Status == GetInventoryStatus.Success;

    public static GetInventoryResult Success(
        IEnumerable<InventoryItemResult> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        return new GetInventoryResult(
            GetInventoryStatus.Success,
            items.ToArray());
    }

    public static GetInventoryResult InventoryNotFound() =>
        new(
            GetInventoryStatus.InventoryNotFound,
            Array.Empty<InventoryItemResult>());
}
