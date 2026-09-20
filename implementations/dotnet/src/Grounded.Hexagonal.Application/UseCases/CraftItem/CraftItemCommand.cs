using Grounded.Hexagonal.Domain.Crafting;
using Grounded.Hexagonal.Domain.Inventory;

namespace Grounded.Hexagonal.Application.UseCases.CraftItem;

/// <summary>
/// Input model for UC-CRAFT-001.
/// </summary>
/// <remarks>
/// This is an Application model, not an HTTP request DTO.
/// </remarks>
public sealed record CraftItemCommand
{
    public CraftItemCommand(
        PlayerId playerId,
        RecipeId recipeId)
    {
        ArgumentNullException.ThrowIfNull(playerId);
        ArgumentNullException.ThrowIfNull(recipeId);

        PlayerId = playerId;
        RecipeId = recipeId;
    }

    public PlayerId PlayerId { get; }

    public RecipeId RecipeId { get; }
}
