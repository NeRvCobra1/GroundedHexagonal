namespace Grounded.Hexagonal.Adapters.Inbound.Http.GetInventory;

/// <summary>
/// HTTP representation of one inventory item.
/// </summary>
public sealed record InventoryItemHttpResponse(
    Guid ItemId,
    int Quantity);

/// <summary>
/// Successful HTTP representation returned by the GetInventory endpoint.
/// </summary>
public sealed record GetInventoryHttpResponse(
    Guid PlayerId,
    IReadOnlyCollection<InventoryItemHttpResponse> Items);

/// <summary>
/// Error representation returned by the GetInventory HTTP adapter.
/// </summary>
public sealed record GetInventoryErrorHttpResponse(
    string Status);
