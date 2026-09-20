using Grounded.Hexagonal.Domain.Crafting;

namespace Grounded.Hexagonal.Application.Ports.Outbound;

/// <summary>
/// Outbound port used by Application to obtain crafting recipes.
/// </summary>
/// <remarks>
/// Architecture ID: PORT-OUT-RECIPE-001
///
/// The contract does not define whether recipes come from a database,
/// memory, a file or another external system.
/// </remarks>
public interface IRecipeRepository
{
    Task<Recipe?> GetByIdAsync(
        RecipeId recipeId,
        CancellationToken cancellationToken = default);
}
