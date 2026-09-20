using Grounded.Hexagonal.Adapters.Outbound.Persistence.InMemory;
using Grounded.Hexagonal.Domain.Crafting;
using Grounded.Hexagonal.Domain.Inventory;
using Grounded.Hexagonal.Domain.Items;
using InventoryAggregate = Grounded.Hexagonal.Domain.Inventory.Inventory;

namespace Grounded.Hexagonal.Persistence.IntegrationTests.InMemory;

public sealed class InMemoryPersistenceTests
{
    [Fact]
    public async Task InventoryRepository_Should_Return_Seeded_Inventory()
    {
        var playerId = PlayerId.New();
        var inventory = new InventoryAggregate(playerId);

        var repository = new InMemoryInventoryRepository([inventory]);

        var result = await repository.GetByPlayerIdAsync(playerId);

        Assert.Same(inventory, result);
    }

    [Fact]
    public async Task InventoryRepository_Should_Save_New_Inventory()
    {
        var playerId = PlayerId.New();
        var inventory = new InventoryAggregate(playerId);

        var repository = new InMemoryInventoryRepository();

        await repository.SaveAsync(inventory);

        var result = await repository.GetByPlayerIdAsync(playerId);

        Assert.Same(inventory, result);
    }

    [Fact]
    public async Task RecipeRepository_Should_Return_Seeded_Recipe()
    {
        var recipe = new Recipe(
            RecipeId.New(),
            ItemId.New(),
            1,
            [
                new IngredientRequirement(ItemId.New(), 2)
            ]);

        var repository = new InMemoryRecipeRepository([recipe]);

        var result = await repository.GetByIdAsync(recipe.Id);

        Assert.Same(recipe, result);
    }
}
