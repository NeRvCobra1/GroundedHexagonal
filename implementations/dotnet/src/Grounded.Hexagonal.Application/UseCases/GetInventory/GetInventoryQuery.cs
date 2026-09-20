using Grounded.Hexagonal.Domain.Inventory;

namespace Grounded.Hexagonal.Application.UseCases.GetInventory;

/// <summary>
/// Input model for UC-INVENTORY-001.
/// </summary>
/// <remarks>
/// This is an Application query, not an HTTP request model.
/// </remarks>
public sealed record GetInventoryQuery
{
    public GetInventoryQuery(PlayerId playerId)
    {
        ArgumentNullException.ThrowIfNull(playerId);
        PlayerId = playerId;
    }

    public PlayerId PlayerId { get; }
}
