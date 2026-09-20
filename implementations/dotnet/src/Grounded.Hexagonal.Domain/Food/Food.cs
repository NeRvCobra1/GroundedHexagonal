namespace Grounded.Hexagonal.Domain.Food;

/// <summary>
/// Represents one perishable food instance.
/// </summary>
/// <remarks>
/// Architecture rule:
/// RULE-SPOILAGE-001 — a fresh food becomes spoiled when the current time
/// reaches or passes its spoilage instant. An already spoiled food remains
/// spoiled.
/// </remarks>
public sealed class Food
{
    public Food(
        FoodId id,
        DateTimeOffset spoilsAt)
    {
        ArgumentNullException.ThrowIfNull(id);

        Id = id;
        SpoilsAt = spoilsAt;
        State = FoodSpoilageState.Fresh;
    }

    public FoodId Id { get; }

    public DateTimeOffset SpoilsAt { get; }

    public FoodSpoilageState State { get; private set; }

    public bool IsSpoiled => State == FoodSpoilageState.Spoiled;

    /// <summary>
    /// Advances the spoilage state using a time supplied from outside Domain.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> only when this call changes the state from
    /// Fresh to Spoiled.
    /// </returns>
    public bool AdvanceSpoilage(DateTimeOffset currentTime)
    {
        if (IsSpoiled || currentTime < SpoilsAt)
        {
            return false;
        }

        State = FoodSpoilageState.Spoiled;
        return true;
    }
}
