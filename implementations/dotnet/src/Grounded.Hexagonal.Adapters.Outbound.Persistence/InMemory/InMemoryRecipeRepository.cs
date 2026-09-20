using System.Collections.Concurrent;
using Grounded.Hexagonal.Application.Ports.Outbound;
using Grounded.Hexagonal.Domain.Crafting;

namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.InMemory;

/// <summary>
/// In-memory implementation of PORT-OUT-RECIPE-001.
/// </summary>
/// <remarks>
/// This outbound adapter stores recipes in process memory.
///
/// Application depends only on <see cref="IRecipeRepository"/> and therefore
/// does not know that this particular implementation uses a dictionary.
/// </remarks>
public sealed class InMemoryRecipeRepository : IRecipeRepository
{
    private readonly ConcurrentDictionary<RecipeId, Recipe> _recipes;

    public InMemoryRecipeRepository()
        : this(Array.Empty<Recipe>())
    {
    }

    public InMemoryRecipeRepository(IEnumerable<Recipe> recipes)
    {
        ArgumentNullException.ThrowIfNull(recipes);

        _recipes = new ConcurrentDictionary<RecipeId, Recipe>(
            recipes.ToDictionary(
                recipe => recipe.Id,
                recipe => recipe));
    }

    public Task<Recipe?> GetByIdAsync(
        RecipeId recipeId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(recipeId);
        cancellationToken.ThrowIfCancellationRequested();

        _recipes.TryGetValue(recipeId, out var recipe);

        return Task.FromResult(recipe);
    }
}
