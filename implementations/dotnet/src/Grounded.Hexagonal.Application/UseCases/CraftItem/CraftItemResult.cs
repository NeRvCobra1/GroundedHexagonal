namespace Grounded.Hexagonal.Application.UseCases.CraftItem;

/// <summary>
/// Possible outcomes of UC-CRAFT-001.
/// </summary>
public enum CraftItemStatus
{
    Success,
    RecipeNotFound,
    InventoryNotFound,
    InsufficientIngredients
}

/// <summary>
/// Application-level result for UC-CRAFT-001.
/// </summary>
/// <remarks>
/// This result intentionally contains no HTTP status code or transport detail.
/// Inbound adapters are responsible for translating it to their own protocol.
/// </remarks>
public sealed record CraftItemResult
{
    private CraftItemResult(CraftItemStatus status)
    {
        Status = status;
    }

    public CraftItemStatus Status { get; }

    public bool IsSuccess => Status == CraftItemStatus.Success;

    public static CraftItemResult Success() =>
        new(CraftItemStatus.Success);

    public static CraftItemResult RecipeNotFound() =>
        new(CraftItemStatus.RecipeNotFound);

    public static CraftItemResult InventoryNotFound() =>
        new(CraftItemStatus.InventoryNotFound);

    public static CraftItemResult InsufficientIngredients() =>
        new(CraftItemStatus.InsufficientIngredients);
}
