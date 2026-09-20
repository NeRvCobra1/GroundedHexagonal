using Grounded.Hexagonal.Application.Ports.Inbound;
using Grounded.Hexagonal.Application.Ports.Outbound;
using Grounded.Hexagonal.Domain.Inventory;

namespace Grounded.Hexagonal.Application.UseCases.CraftItem;

/// <summary>
/// Coordinates UC-CRAFT-001.
/// </summary>
/// <remarks>
/// Architecture ID: UC-CRAFT-001
///
/// The Handler coordinates ports and delegates crafting rules to Domain.
/// It contains no HTTP, persistence technology or recipe-specific logic.
/// </remarks>
public sealed class CraftItemHandler : ICraftItemUseCase
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IRecipeRepository _recipeRepository;

    public CraftItemHandler(
        IInventoryRepository inventoryRepository,
        IRecipeRepository recipeRepository)
    {
        ArgumentNullException.ThrowIfNull(inventoryRepository);
        ArgumentNullException.ThrowIfNull(recipeRepository);

        _inventoryRepository = inventoryRepository;
        _recipeRepository = recipeRepository;
    }

    public async Task<CraftItemResult> ExecuteAsync(
        CraftItemCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var recipe = await _recipeRepository.GetByIdAsync(
            command.RecipeId,
            cancellationToken);

        if (recipe is null)
        {
            return CraftItemResult.RecipeNotFound();
        }

        var inventory = await _inventoryRepository.GetByPlayerIdAsync(
            command.PlayerId,
            cancellationToken);

        if (inventory is null)
        {
            return CraftItemResult.InventoryNotFound();
        }

        try
        {
            inventory.Craft(recipe);
        }
        catch (InsufficientIngredientsException)
        {
            return CraftItemResult.InsufficientIngredients();
        }

        await _inventoryRepository.SaveAsync(
            inventory,
            cancellationToken);

        return CraftItemResult.Success();
    }
}
