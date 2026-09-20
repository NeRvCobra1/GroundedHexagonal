namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore.Models;

internal sealed class RecipeIngredientRecord
{
    public Guid RecipeId { get; set; }

    public int Position { get; set; }

    public Guid ItemId { get; set; }

    public int Quantity { get; set; }

    public RecipeRecord Recipe { get; set; } = null!;
}
