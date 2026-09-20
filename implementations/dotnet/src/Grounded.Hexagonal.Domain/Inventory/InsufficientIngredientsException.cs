using Grounded.Hexagonal.Domain.Crafting;

namespace Grounded.Hexagonal.Domain.Inventory;

/// <summary>
/// Domain error raised when an inventory cannot satisfy all requirements
/// of a recipe.
/// </summary>
public sealed class InsufficientIngredientsException : Exception
{
    public InsufficientIngredientsException(RecipeId recipeId)
        : base($"The inventory does not contain enough ingredients for recipe '{recipeId}'.")
    {
        RecipeId = recipeId;
    }

    public RecipeId RecipeId { get; }
}
