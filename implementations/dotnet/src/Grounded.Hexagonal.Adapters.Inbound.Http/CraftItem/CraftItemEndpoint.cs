using Grounded.Hexagonal.Application.Ports.Inbound;
using Grounded.Hexagonal.Application.UseCases.CraftItem;
using Grounded.Hexagonal.Domain.Crafting;
using Grounded.Hexagonal.Domain.Inventory;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Grounded.Hexagonal.Adapters.Inbound.Http.CraftItem;

/// <summary>
/// HTTP inbound adapter for UC-CRAFT-001.
/// </summary>
/// <remarks>
/// Architecture ID: ADAPTER-IN-HTTP-CRAFT-001
///
/// This adapter translates HTTP input into the inbound port model and
/// translates Application results back into HTTP responses.
/// </remarks>
public static class CraftItemEndpoint
{
    public static IEndpointRouteBuilder MapCraftItemEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
                "/api/crafting/items",
                HandleAsync)
            .WithName("CraftItem");

        return endpoints;
    }

    private static async Task<IResult> HandleAsync(
        CraftItemHttpRequest request,
        ICraftItemUseCase useCase,
        CancellationToken cancellationToken)
    {
        if (request.PlayerId == Guid.Empty ||
            request.RecipeId == Guid.Empty)
        {
            return Results.BadRequest(
                new CraftItemHttpResponse("InvalidRequest"));
        }

        var command = new CraftItemCommand(
            PlayerId.From(request.PlayerId),
            RecipeId.From(request.RecipeId));

        var result = await useCase.ExecuteAsync(
            command,
            cancellationToken);

        return result.Status switch
        {
            CraftItemStatus.Success =>
                Results.Ok(
                    new CraftItemHttpResponse("Success")),

            CraftItemStatus.RecipeNotFound =>
                Results.NotFound(
                    new CraftItemHttpResponse("RecipeNotFound")),

            CraftItemStatus.InventoryNotFound =>
                Results.NotFound(
                    new CraftItemHttpResponse("InventoryNotFound")),

            CraftItemStatus.InsufficientIngredients =>
                Results.Conflict(
                    new CraftItemHttpResponse("InsufficientIngredients")),

            _ => throw new InvalidOperationException(
                $"Unsupported CraftItem status '{result.Status}'.")
        };
    }
}
