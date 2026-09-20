using System.Net;
using System.Net.Http.Json;
using Grounded.Hexagonal.Adapters.Outbound.Persistence.InMemory;
using Grounded.Hexagonal.Application.Ports.Outbound;
using Grounded.Hexagonal.Domain.Crafting;
using Grounded.Hexagonal.Domain.Inventory;
using Grounded.Hexagonal.Domain.Items;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using InventoryAggregate = Grounded.Hexagonal.Domain.Inventory.Inventory;

namespace Grounded.Hexagonal.Http.IntegrationTests.CraftItem;

public sealed class CraftItemHttpTests
{
    [Fact]
    public async Task Post_Should_Return_200_When_Crafting_Succeeds()
    {
        var scenario = CraftScenario.Success();

        await using var factory = CreateFactory(scenario);
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/crafting/items",
            new
            {
                playerId = scenario.PlayerId.Value,
                recipeId = scenario.Recipe.Id.Value
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<HttpResponseContract>();

        Assert.NotNull(body);
        Assert.Equal("Success", body.Status);

        var persistedInventory =
            await scenario.InventoryRepository.GetByPlayerIdAsync(
                scenario.PlayerId);

        Assert.NotNull(persistedInventory);
        Assert.Equal(
            1,
            persistedInventory.GetQuantity(scenario.ResultItemId));
    }

    [Fact]
    public async Task Post_Should_Return_404_When_Recipe_Does_Not_Exist()
    {
        var scenario = CraftScenario.WithoutRecipe();

        await using var factory = CreateFactory(scenario);
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/crafting/items",
            new
            {
                playerId = scenario.PlayerId.Value,
                recipeId = scenario.RequestedRecipeId.Value
            });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<HttpResponseContract>();

        Assert.NotNull(body);
        Assert.Equal("RecipeNotFound", body.Status);
    }

    [Fact]
    public async Task Post_Should_Return_404_When_Inventory_Does_Not_Exist()
    {
        var scenario = CraftScenario.WithoutInventory();

        await using var factory = CreateFactory(scenario);
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/crafting/items",
            new
            {
                playerId = scenario.PlayerId.Value,
                recipeId = scenario.Recipe.Id.Value
            });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<HttpResponseContract>();

        Assert.NotNull(body);
        Assert.Equal("InventoryNotFound", body.Status);
    }

    [Fact]
    public async Task Post_Should_Return_409_When_Ingredients_Are_Insufficient()
    {
        var scenario = CraftScenario.InsufficientIngredients();

        await using var factory = CreateFactory(scenario);
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/crafting/items",
            new
            {
                playerId = scenario.PlayerId.Value,
                recipeId = scenario.Recipe.Id.Value
            });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<HttpResponseContract>();

        Assert.NotNull(body);
        Assert.Equal("InsufficientIngredients", body.Status);
    }

    [Fact]
    public async Task Post_Should_Return_400_When_Request_Contains_Empty_Identifier()
    {
        var scenario = CraftScenario.Success();

        await using var factory = CreateFactory(scenario);
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/crafting/items",
            new
            {
                playerId = Guid.Empty,
                recipeId = scenario.Recipe.Id.Value
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<HttpResponseContract>();

        Assert.NotNull(body);
        Assert.Equal("InvalidRequest", body.Status);
    }

    private static WebApplicationFactory<Program> CreateFactory(
        CraftScenario scenario)
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");

                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IInventoryRepository>();
                    services.RemoveAll<IRecipeRepository>();

                    services.AddSingleton<IInventoryRepository>(
                        scenario.InventoryRepository);

                    services.AddSingleton<IRecipeRepository>(
                        scenario.RecipeRepository);
                });
            });
    }

    private sealed record HttpResponseContract(string Status);

    private sealed class CraftScenario
    {
        private CraftScenario(
            PlayerId playerId,
            Recipe recipe,
            RecipeId requestedRecipeId,
            ItemId resultItemId,
            InMemoryInventoryRepository inventoryRepository,
            InMemoryRecipeRepository recipeRepository)
        {
            PlayerId = playerId;
            Recipe = recipe;
            RequestedRecipeId = requestedRecipeId;
            ResultItemId = resultItemId;
            InventoryRepository = inventoryRepository;
            RecipeRepository = recipeRepository;
        }

        public PlayerId PlayerId { get; }

        public Recipe Recipe { get; }

        public RecipeId RequestedRecipeId { get; }

        public ItemId ResultItemId { get; }

        public InMemoryInventoryRepository InventoryRepository { get; }

        public InMemoryRecipeRepository RecipeRepository { get; }

        public static CraftScenario Success()
        {
            return Create(
                includeInventory: true,
                includeRecipe: true,
                hasEnoughIngredients: true);
        }

        public static CraftScenario WithoutRecipe()
        {
            return Create(
                includeInventory: true,
                includeRecipe: false,
                hasEnoughIngredients: true);
        }

        public static CraftScenario WithoutInventory()
        {
            return Create(
                includeInventory: false,
                includeRecipe: true,
                hasEnoughIngredients: true);
        }

        public static CraftScenario InsufficientIngredients()
        {
            return Create(
                includeInventory: true,
                includeRecipe: true,
                hasEnoughIngredients: false);
        }

        private static CraftScenario Create(
            bool includeInventory,
            bool includeRecipe,
            bool hasEnoughIngredients)
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

            if (hasEnoughIngredients)
            {
                inventory.Add(mintShard, 10);
                inventory.Add(toughGunk, 5);
                inventory.Add(flowerPetal, 3);
            }
            else
            {
                inventory.Add(mintShard, 10);
                inventory.Add(toughGunk, 4);
                inventory.Add(flowerPetal, 3);
            }

            var inventoryRepository =
                includeInventory
                    ? new InMemoryInventoryRepository([inventory])
                    : new InMemoryInventoryRepository();

            var recipeRepository =
                includeRecipe
                    ? new InMemoryRecipeRepository([recipe])
                    : new InMemoryRecipeRepository();

            return new CraftScenario(
                playerId,
                recipe,
                recipe.Id,
                mintMace,
                inventoryRepository,
                recipeRepository);
        }
    }
}
