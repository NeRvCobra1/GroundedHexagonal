using Grounded.Hexagonal.Adapters.Outbound.Persistence.InMemory;
using Grounded.Hexagonal.Application.UseCases.CraftItem;
using Grounded.Hexagonal.Domain.Crafting;
using Grounded.Hexagonal.Domain.Inventory;
using Grounded.Hexagonal.Domain.Items;
using InventoryAggregate = Grounded.Hexagonal.Domain.Inventory.Inventory;

namespace Grounded.Hexagonal.Persistence.IntegrationTests.InMemory;

public sealed class CraftItemPersistenceFlowTests
{
    [Fact]
    public async Task CraftItem_Should_Run_Through_Application_Using_InMemory_Outbound_Adapters()
    {
        var playerId = PlayerId.New();

        var mintShard = ItemId.New();
        var toughGunk = ItemId.New();
        var flowerPetal = ItemId.New();
        var mintMace = ItemId.New();

        var recipe = new Recipe(
            RecipeId.New(),
            mintMace,
            1,
            [
                new IngredientRequirement(mintShard, 10),
                new IngredientRequirement(toughGunk, 5),
                new IngredientRequirement(flowerPetal, 3)
            ]);

        var inventory = new InventoryAggregate(playerId);
        inventory.Add(mintShard, 12);
        inventory.Add(toughGunk, 5);
        inventory.Add(flowerPetal, 8);

        var inventoryRepository =
            new InMemoryInventoryRepository([inventory]);

        var recipeRepository =
            new InMemoryRecipeRepository([recipe]);

        var handler = new CraftItemHandler(
            inventoryRepository,
            recipeRepository);

        var result = await handler.ExecuteAsync(
            new CraftItemCommand(playerId, recipe.Id));

        var persistedInventory =
            await inventoryRepository.GetByPlayerIdAsync(playerId);

        Assert.True(result.IsSuccess);
        Assert.NotNull(persistedInventory);

        Assert.Equal(2, persistedInventory.GetQuantity(mintShard));
        Assert.Equal(0, persistedInventory.GetQuantity(toughGunk));
        Assert.Equal(5, persistedInventory.GetQuantity(flowerPetal));
        Assert.Equal(1, persistedInventory.GetQuantity(mintMace));
    }
}
