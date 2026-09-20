using Grounded.Hexagonal.Domain.Items;

namespace Grounded.Hexagonal.Domain.Crafting;

/// <summary>
/// Defines how an item can be crafted.
/// </summary>
/// <remarks>
/// A Recipe is a domain Entity because it has its own identity through
/// <see cref="RecipeId"/>.
///
/// Concrete recipes such as "Mint Mace" are data represented by instances of
/// this type. The crafting algorithm must not hardcode a specific recipe.
/// </remarks>
public sealed class Recipe
{
    private readonly IngredientRequirement[] _ingredients;

    public Recipe(
        RecipeId id,
        ItemId resultItemId,
        int resultQuantity,
        IEnumerable<IngredientRequirement> ingredients)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(resultItemId);
        ArgumentNullException.ThrowIfNull(ingredients);

        if (resultQuantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(resultQuantity),
                resultQuantity,
                "A recipe must produce at least one item.");
        }

        Id = id;
        ResultItemId = resultItemId;
        ResultQuantity = resultQuantity;
        _ingredients = ingredients.ToArray();
    }

    public RecipeId Id { get; }

    public ItemId ResultItemId { get; }

    public int ResultQuantity { get; }

    public IReadOnlyList<IngredientRequirement> Ingredients => _ingredients;
}
