using Grounded.Hexagonal.Application.UseCases.GetInventory;

namespace Grounded.Hexagonal.Application.Ports.Inbound;

/// <summary>
/// Inbound port for the GetInventory use case.
/// </summary>
/// <remarks>
/// Architecture ID: PORT-IN-INVENTORY-001
/// Use Case: UC-INVENTORY-001
/// </remarks>
public interface IGetInventoryUseCase
{
    Task<GetInventoryResult> ExecuteAsync(
        GetInventoryQuery query,
        CancellationToken cancellationToken = default);
}
