using Grounded.Hexagonal.Application.Ports.Outbound;
using Grounded.Hexagonal.Application.UseCases.CraftItem;
using Grounded.Hexagonal.Domain.Crafting;
using Grounded.Hexagonal.Domain.Inventory;
using Grounded.Hexagonal.Domain.Items;
using InventoryAggregate = Grounded.Hexagonal.Domain.Inventory.Inventory;

namespace Grounded.Hexagonal.Application.Tests.CraftItem;

public sealed class CraftItemHandlerTests
{
    [Fact]
    public async Task ExecuteAsync_Should_Return_RecipeNotFound_When_Recipe_Does_Not_Exist()
    {
        var playerId = PlayerId.New();
        var recipeId = RecipeId.New();

        var inventoryRepository = new FakeInventoryRepository(
            new InventoryAggregate(playerId));

        var recipeRepository = new FakeRecipeRepository(null);

        var handler = new CraftItemHandler(
            inventoryRepository,
            recipeRepository);

        var result = await handler.ExecuteAsync(
            new CraftItemCommand(playerId, recipeId));

        Assert.Equal(CraftItemStatus.RecipeNotFound, result.Status);
        Assert.False(result.IsSuccess);
        Assert.Equal(0, inventoryRepository.GetCalls);
        Assert.Equal(0, inventoryRepository.SaveCalls);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_InventoryNotFound_When_Inventory_Does_Not_Exist()
    {
        var playerId = PlayerId.New();
        var recipe = CreateMintMaceRecipe();

        var inventoryRepository = new FakeInventoryRepository(null);
        var recipeRepository = new FakeRecipeRepository(recipe);

        var handler = new CraftItemHandler(
            inventoryRepository,
            recipeRepository);

        var result = await handler.ExecuteAsync(
            new CraftItemCommand(playerId, recipe.Id));

        Assert.Equal(CraftItemStatus.InventoryNotFound, result.Status);
        Assert.False(result.IsSuccess);
        Assert.Equal(1, inventoryRepository.GetCalls);
        Assert.Equal(0, inventoryRepository.SaveCalls);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_InsufficientIngredients_And_Not_Save_When_Domain_Rejects_Craft()
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
        inventory.Add(mintShard, 10);
        inventory.Add(toughGunk, 4);
        inventory.Add(flowerPetal, 3);

        var inventoryRepository = new FakeInventoryRepository(inventory);
        var recipeRepository = new FakeRecipeRepository(recipe);

        var handler = new CraftItemHandler(
            inventoryRepository,
            recipeRepository);

        var result = await handler.ExecuteAsync(
            new CraftItemCommand(playerId, recipe.Id));

        Assert.Equal(CraftItemStatus.InsufficientIngredients, result.Status);
        Assert.False(result.IsSuccess);
        Assert.Equal(0, inventoryRepository.SaveCalls);

        Assert.Equal(10, inventory.GetQuantity(mintShard));
        Assert.Equal(4, inventory.GetQuantity(toughGunk));
        Assert.Equal(3, inventory.GetQuantity(flowerPetal));
        Assert.Equal(0, inventory.GetQuantity(mintMace));
    }

    [Fact]
    public async Task ExecuteAsync_Should_Craft_And_Save_Inventory_When_Request_Is_Valid()
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

        var inventoryRepository = new FakeInventoryRepository(inventory);
        var recipeRepository = new FakeRecipeRepository(recipe);

        var handler = new CraftItemHandler(
            inventoryRepository,
            recipeRepository);

        var result = await handler.ExecuteAsync(
            new CraftItemCommand(playerId, recipe.Id));

        Assert.Equal(CraftItemStatus.Success, result.Status);
        Assert.True(result.IsSuccess);
        Assert.Equal(1, inventoryRepository.SaveCalls);
        Assert.Same(inventory, inventoryRepository.LastSavedInventory);

        Assert.Equal(2, inventory.GetQuantity(mintShard));
        Assert.Equal(0, inventory.GetQuantity(toughGunk));
        Assert.Equal(5, inventory.GetQuantity(flowerPetal));
        Assert.Equal(1, inventory.GetQuantity(mintMace));
    }

    private static Recipe CreateMintMaceRecipe()
    {
        return new Recipe(
            RecipeId.New(),
            ItemId.New(),
            1,
            [
                new IngredientRequirement(ItemId.New(), 10),
                new IngredientRequirement(ItemId.New(), 5),
                new IngredientRequirement(ItemId.New(), 3)
            ]);
    }

    private sealed class FakeInventoryRepository : IInventoryRepository
    {
        private readonly InventoryAggregate? _inventory;

        public FakeInventoryRepository(InventoryAggregate? inventory)
        {
            _inventory = inventory;
        }

        public int GetCalls { get; private set; }

        public int SaveCalls { get; private set; }

        public InventoryAggregate? LastSavedInventory { get; private set; }

        public Task<InventoryAggregate?> GetByPlayerIdAsync(
            PlayerId playerId,
            CancellationToken cancellationToken = default)
        {
            GetCalls++;
            return Task.FromResult(_inventory);
        }

        public Task SaveAsync(
            InventoryAggregate inventory,
            CancellationToken cancellationToken = default)
        {
            SaveCalls++;
            LastSavedInventory = inventory;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeRecipeRepository : IRecipeRepository
    {
        private readonly Recipe? _recipe;

        public FakeRecipeRepository(Recipe? recipe)
        {
            _recipe = recipe;
        }

        public Task<Recipe?> GetByIdAsync(
            RecipeId recipeId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_recipe);
        }
    }
}
