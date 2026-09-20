using Grounded.Hexagonal.Domain.Crafting;
using Grounded.Hexagonal.Domain.Items;

namespace Grounded.Hexagonal.Domain.Tests.Crafting;

public sealed class RecipeTests
{
    [Fact]
    public void IngredientRequirement_Should_Reject_Non_Positive_Quantity()
    {
        var itemId = ItemId.New();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => new IngredientRequirement(itemId, 0));
    }

    [Fact]
    public void Recipe_Should_Reject_Non_Positive_Result_Quantity()
    {
        var recipeId = RecipeId.New();
        var resultItemId = ItemId.New();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Recipe(
                recipeId,
                resultItemId,
                0,
                Array.Empty<IngredientRequirement>()));
    }
}
