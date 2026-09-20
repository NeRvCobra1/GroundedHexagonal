namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore.Models;

internal sealed class RecipeRecord
{
    public Guid RecipeId { get; set; }

    public Guid ResultItemId { get; set; }

    public int ResultQuantity { get; set; }

    public List<RecipeIngredientRecord> Ingredients { get; set; } = [];
}
