using Grounded.Hexagonal.Domain.Crafting;
using Grounded.Hexagonal.Domain.Items;

namespace Grounded.Hexagonal.Domain.Inventory;

/// <summary>
/// Represents the inventory owned by a player.
/// </summary>
/// <remarks>
/// This is a domain Entity because it is associated with a specific
/// <see cref="PlayerId"/>.
///
/// Crafting behavior in this type implements:
/// RULE-CRAFT-001 — validate every required material before mutation.
/// RULE-CRAFT-002 — consume required materials and add the crafted result.
/// </remarks>
public sealed class Inventory
{
    private readonly Dictionary<ItemId, int> _quantities = [];

    public Inventory(PlayerId playerId)
    {
        ArgumentNullException.ThrowIfNull(playerId);
        PlayerId = playerId;
    }

    public PlayerId PlayerId { get; }

    public int GetQuantity(ItemId itemId)
    {
        ArgumentNullException.ThrowIfNull(itemId);

        return _quantities.GetValueOrDefault(itemId);
    }

    /// <summary>
    /// Returns a read-only snapshot of the current inventory contents.
    /// </summary>
    /// <remarks>
    /// The internal dictionary is intentionally not exposed.
    /// Callers receive domain values representing the state at the time
    /// this method is executed.
    /// </remarks>
    public IReadOnlyCollection<InventoryItemQuantity> GetItems()
    {
        return _quantities
            .OrderBy(pair => pair.Key.Value)
            .Select(pair => new InventoryItemQuantity(
                pair.Key,
                pair.Value))
            .ToArray();
    }

    public void Add(ItemId itemId, int quantity)
    {
        ArgumentNullException.ThrowIfNull(itemId);

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                quantity,
                "The quantity added to an inventory must be greater than zero.");
        }

        _quantities[itemId] = GetQuantity(itemId) + quantity;
    }

    /// <summary>
    /// Applies the crafting rules for the supplied recipe.
    /// </summary>
    /// <remarks>
    /// Architecture rules:
    /// RULE-CRAFT-001 — all requirements are validated before any mutation.
    /// RULE-CRAFT-002 — a successful craft consumes ingredients and adds
    /// the recipe result.
    /// </remarks>
    public void Craft(Recipe recipe)
    {
        ArgumentNullException.ThrowIfNull(recipe);

        var requirements = recipe.Ingredients
            .GroupBy(requirement => requirement.ItemId)
            .Select(group => new
            {
                ItemId = group.Key,
                Quantity = group.Sum(requirement => requirement.Quantity)
            })
            .ToArray();

        var hasInsufficientIngredients = requirements.Any(
            requirement => GetQuantity(requirement.ItemId) < requirement.Quantity);

        if (hasInsufficientIngredients)
        {
            throw new InsufficientIngredientsException(recipe.Id);
        }

        foreach (var requirement in requirements)
        {
            Remove(requirement.ItemId, requirement.Quantity);
        }

        Add(recipe.ResultItemId, recipe.ResultQuantity);
    }

    private void Remove(ItemId itemId, int quantity)
    {
        var remaining = GetQuantity(itemId) - quantity;

        if (remaining == 0)
        {
            _quantities.Remove(itemId);
            return;
        }

        _quantities[itemId] = remaining;
    }
}
