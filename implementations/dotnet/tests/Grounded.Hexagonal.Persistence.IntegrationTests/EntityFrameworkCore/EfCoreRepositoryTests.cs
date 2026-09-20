using Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore;
using Grounded.Hexagonal.Domain.Crafting;
using Grounded.Hexagonal.Domain.Food;
using Grounded.Hexagonal.Domain.Inventory;
using Grounded.Hexagonal.Domain.Items;
using InventoryAggregate = Grounded.Hexagonal.Domain.Inventory.Inventory;

namespace Grounded.Hexagonal.Persistence.IntegrationTests.EntityFrameworkCore;

public sealed class EfCoreRepositoryTests
{
    [Fact]
    public async Task InventoryRepository_Should_Persist_Across_Repository_Instances()
    {
        await using var database =
            await SqliteTestDatabase.CreateAsync();

        var playerId = PlayerId.New();
        var itemId = ItemId.New();

        var inventory = new InventoryAggregate(playerId);
        inventory.Add(itemId, 7);

        var writer = new EfCoreInventoryRepository(
            database.PersistenceOptions);

        await writer.SaveAsync(inventory);

        var reader = new EfCoreInventoryRepository(
            database.PersistenceOptions);

        var restored =
            await reader.GetByPlayerIdAsync(playerId);

        Assert.NotNull(restored);
        Assert.Equal(7, restored.GetQuantity(itemId));
    }

    [Fact]
    public async Task RecipeRepository_Should_Preserve_Duplicate_Requirements()
    {
        await using var database =
            await SqliteTestDatabase.CreateAsync();

        var repeatedItem = ItemId.New();

        var recipe = new Recipe(
            RecipeId.New(),
            ItemId.New(),
            1,
            [
                new IngredientRequirement(repeatedItem, 2),
                new IngredientRequirement(repeatedItem, 3)
            ]);

        var seeder = new EfCoreDataSeeder(
            database.PersistenceOptions);

        await seeder.SeedRecipeAsync(recipe);

        var repository = new EfCoreRecipeRepository(
            database.PersistenceOptions);

        var restored =
            await repository.GetByIdAsync(recipe.Id);

        Assert.NotNull(restored);
        Assert.Equal(2, restored.Ingredients.Count);
        Assert.Equal(2, restored.Ingredients[0].Quantity);
        Assert.Equal(3, restored.Ingredients[1].Quantity);
    }

    [Fact]
    public async Task FoodRepository_Should_Preserve_Spoiled_State()
    {
        await using var database =
            await SqliteTestDatabase.CreateAsync();

        var spoilsAt = new DateTimeOffset(
            2026, 9, 20, 12, 0, 0, TimeSpan.Zero);

        var food = new Domain.Food.Food(
            FoodId.New(),
            spoilsAt);

        food.AdvanceSpoilage(spoilsAt);

        var writer = new EfCoreFoodRepository(
            database.PersistenceOptions);

        await writer.SaveAsync(food);

        var reader = new EfCoreFoodRepository(
            database.PersistenceOptions);

        var restored = Assert.Single(
            await reader.GetAllAsync());

        Assert.Equal(food.Id, restored.Id);
        Assert.Equal(spoilsAt, restored.SpoilsAt);
        Assert.True(restored.IsSpoiled);
    }
}
