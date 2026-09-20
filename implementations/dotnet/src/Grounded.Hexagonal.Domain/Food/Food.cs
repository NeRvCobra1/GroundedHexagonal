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
        : this(
            id,
            spoilsAt,
            FoodSpoilageState.Fresh)
    {
    }

    private Food(
        FoodId id,
        DateTimeOffset spoilsAt,
        FoodSpoilageState state)
    {
        ArgumentNullException.ThrowIfNull(id);

        if (!Enum.IsDefined(typeof(FoodSpoilageState), state))
        {
            throw new ArgumentOutOfRangeException(
                nameof(state),
                state,
                "The persisted food state is not valid.");
        }

        Id = id;
        SpoilsAt = spoilsAt;
        State = state;
    }

    public FoodId Id { get; }

    public DateTimeOffset SpoilsAt { get; }

    public FoodSpoilageState State { get; private set; }

    public bool IsSpoiled => State == FoodSpoilageState.Spoiled;

    /// <summary>
    /// Rehydrates a food instance from persisted domain state.
    /// </summary>
    /// <remarks>
    /// This factory is technology independent. It allows an outbound adapter
    /// to reconstruct the domain entity without replaying the spoilage rule.
    /// </remarks>
    public static Food Restore(
        FoodId id,
        DateTimeOffset spoilsAt,
        FoodSpoilageState state)
    {
        return new Food(
            id,
            spoilsAt,
            state);
    }

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
