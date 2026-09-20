namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore.Models;

internal sealed class FoodRecord
{
    public Guid FoodId { get; set; }

    public DateTime SpoilsAtUtc { get; set; }

    public int State { get; set; }
}
