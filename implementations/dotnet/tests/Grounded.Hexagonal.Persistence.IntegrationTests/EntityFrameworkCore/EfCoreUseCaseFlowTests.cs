using Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore;
using Grounded.Hexagonal.Application.Ports.Outbound;
using Grounded.Hexagonal.Application.UseCases.CraftItem;
using Grounded.Hexagonal.Application.UseCases.GetInventory;
using Grounded.Hexagonal.Application.UseCases.ProcessFoodSpoilage;
using Grounded.Hexagonal.Domain.Crafting;
using Grounded.Hexagonal.Domain.Food;
using Grounded.Hexagonal.Domain.Inventory;
using Grounded.Hexagonal.Domain.Items;
using InventoryAggregate = Grounded.Hexagonal.Domain.Inventory.Inventory;

namespace Grounded.Hexagonal.Persistence.IntegrationTests.EntityFrameworkCore;

public sealed class EfCoreUseCaseFlowTests
{
    [Fact]
    public async Task CraftItem_Should_Run_Using_Sqlite_Adapters()
    {
        await using var database =
            await SqliteTestDatabase.CreateAsync();

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

        var seeder = new EfCoreDataSeeder(
            database.PersistenceOptions);

        await seeder.SeedRecipeAsync(recipe);
        await seeder.SeedInventoryAsync(inventory);

        var inventoryRepository =
            new EfCoreInventoryRepository(
                database.PersistenceOptions);

        var recipeRepository =
            new EfCoreRecipeRepository(
                database.PersistenceOptions);

        var handler = new CraftItemHandler(
            inventoryRepository,
            recipeRepository);

        var result = await handler.ExecuteAsync(
            new CraftItemCommand(
                playerId,
                recipe.Id));

        var restored =
            await new EfCoreInventoryRepository(
                database.PersistenceOptions)
                .GetByPlayerIdAsync(playerId);

        Assert.True(result.IsSuccess);
        Assert.NotNull(restored);

        Assert.Equal(2, restored.GetQuantity(mintShard));
        Assert.Equal(0, restored.GetQuantity(toughGunk));
        Assert.Equal(5, restored.GetQuantity(flowerPetal));
        Assert.Equal(1, restored.GetQuantity(mintMace));
    }

    [Fact]
    public async Task GetInventory_Should_Run_Using_Sqlite_Adapter_Without_Saving()
    {
        await using var database =
            await SqliteTestDatabase.CreateAsync();

        var playerId = PlayerId.New();
        var itemId = ItemId.New();

        var inventory = new InventoryAggregate(playerId);
        inventory.Add(itemId, 4);

        var seeder = new EfCoreDataSeeder(
            database.PersistenceOptions);

        await seeder.SeedInventoryAsync(inventory);

        var repository = new EfCoreInventoryRepository(
            database.PersistenceOptions);

        var handler = new GetInventoryHandler(repository);

        var result = await handler.ExecuteAsync(
            new GetInventoryQuery(playerId));

        var item = Assert.Single(result.Items);

        Assert.True(result.IsSuccess);
        Assert.Equal(itemId, item.ItemId);
        Assert.Equal(4, item.Quantity);
    }

    [Fact]
    public async Task ProcessFoodSpoilage_Should_Run_Using_Sqlite_Adapter()
    {
        await using var database =
            await SqliteTestDatabase.CreateAsync();

        var now = new DateTimeOffset(
            2026, 9, 20, 12, 0, 0, TimeSpan.Zero);

        var dueFood = new Domain.Food.Food(
            FoodId.New(),
            now);

        var freshFood = new Domain.Food.Food(
            FoodId.New(),
            now.AddMinutes(5));

        var seeder = new EfCoreDataSeeder(
            database.PersistenceOptions);

        await seeder.SeedFoodAsync(dueFood);
        await seeder.SeedFoodAsync(freshFood);

        var repository = new EfCoreFoodRepository(
            database.PersistenceOptions);

        var handler = new ProcessFoodSpoilageHandler(
            repository,
            new FixedClock(now));

        var result = await handler.ExecuteAsync(
            new ProcessFoodSpoilageCommand());

        var persistedFoods =
            await new EfCoreFoodRepository(
                database.PersistenceOptions)
                .GetAllAsync();

        var persistedDueFood = Assert.Single(
            persistedFoods,
            food => food.Id == dueFood.Id);

        var persistedFreshFood = Assert.Single(
            persistedFoods,
            food => food.Id == freshFood.Id);

        Assert.Equal(2, result.EvaluatedCount);
        Assert.Equal(1, result.SpoiledCount);
        Assert.True(persistedDueFood.IsSpoiled);
        Assert.False(persistedFreshFood.IsSpoiled);
    }

    private sealed class FixedClock : IClock
    {
        public FixedClock(DateTimeOffset utcNow)
        {
            UtcNow = utcNow;
        }

        public DateTimeOffset UtcNow { get; }
    }
}
