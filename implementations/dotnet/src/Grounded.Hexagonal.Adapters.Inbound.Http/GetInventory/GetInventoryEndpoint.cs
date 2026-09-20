using Grounded.Hexagonal.Application.Ports.Inbound;
using Grounded.Hexagonal.Application.UseCases.GetInventory;
using Grounded.Hexagonal.Domain.Inventory;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Grounded.Hexagonal.Adapters.Inbound.Http.GetInventory;

/// <summary>
/// HTTP inbound adapter for UC-INVENTORY-001.
/// </summary>
/// <remarks>
/// Architecture ID: ADAPTER-IN-HTTP-INVENTORY-001
///
/// This adapter translates an HTTP route value into the Application query
/// and translates the Application result back into an HTTP response.
/// </remarks>
public static class GetInventoryEndpoint
{
    public static IEndpointRouteBuilder MapGetInventoryEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
                "/api/inventories/{playerId}",
                HandleAsync)
            .WithName("GetInventory");

        return endpoints;
    }

    private static async Task<IResult> HandleAsync(
        Guid playerId,
        IGetInventoryUseCase useCase,
        CancellationToken cancellationToken)
    {
        if (playerId == Guid.Empty)
        {
            return Results.BadRequest(
                new GetInventoryErrorHttpResponse("InvalidPlayerId"));
        }

        var query = new GetInventoryQuery(
            PlayerId.From(playerId));

        var result = await useCase.ExecuteAsync(
            query,
            cancellationToken);

        return result.Status switch
        {
            GetInventoryStatus.Success =>
                Results.Ok(
                    new GetInventoryHttpResponse(
                        playerId,
                        result.Items
                            .Select(item => new InventoryItemHttpResponse(
                                item.ItemId.Value,
                                item.Quantity))
                            .ToArray())),

            GetInventoryStatus.InventoryNotFound =>
                Results.NotFound(
                    new GetInventoryErrorHttpResponse("InventoryNotFound")),

            _ => throw new InvalidOperationException(
                $"Unsupported GetInventory status '{result.Status}'.")
        };
    }
}
