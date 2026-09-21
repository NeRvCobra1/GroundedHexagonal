using System.Net;
using System.Net.Http.Json;
using Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore;
using Grounded.Hexagonal.Adapters.Outbound.Persistence.InMemory;
using Grounded.Hexagonal.Application.Ports.Outbound;
using Grounded.Hexagonal.Domain.Inventory;
using Grounded.Hexagonal.Domain.Items;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using InventoryAggregate = Grounded.Hexagonal.Domain.Inventory.Inventory;

namespace Grounded.Hexagonal.Http.IntegrationTests.Composition;

public sealed class PersistenceCompositionHttpTests
{
    [Fact]
    public void Api_Host_Should_Default_To_InMemory_Persistence()
    {
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
            });

        using var client = factory.CreateClient();

        var inventoryRepository =
            factory.Services.GetRequiredService<IInventoryRepository>();

        var recipeRepository =
            factory.Services.GetRequiredService<IRecipeRepository>();

        Assert.IsType<InMemoryInventoryRepository>(inventoryRepository);
        Assert.IsType<InMemoryRecipeRepository>(recipeRepository);
    }

    [Fact]
    public async Task Api_Host_Should_Use_Sqlite_Adapters_When_Configured()
    {
        var databasePath = Path.Combine(
            Path.GetTempPath(),
            $"grounded-host-api-{Guid.NewGuid():N}.db");

        var connectionString =
            $"Data Source={databasePath};Pooling=False";

        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.UseSetting(
                    "Persistence:Provider",
                    "Sqlite");
                builder.UseSetting(
                    "ConnectionStrings:Grounded",
                    connectionString);
            });

        try
        {
            using var client = factory.CreateClient();

            var configuredRepository =
                factory.Services.GetRequiredService<IInventoryRepository>();

            Assert.IsType<EfCoreInventoryRepository>(
                configuredRepository);

            var playerId = PlayerId.New();
            var itemId = ItemId.New();

            var inventory = new InventoryAggregate(playerId);
            inventory.Add(itemId, 7);

            var persistenceOptions =
                new SqlitePersistenceOptions(connectionString);

            var seeder =
                new EfCoreDataSeeder(persistenceOptions);

            await seeder.SeedInventoryAsync(inventory);

            var response = await client.GetAsync(
                $"/api/inventories/{playerId.Value}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body =
                await response.Content.ReadFromJsonAsync<GetInventoryResponseContract>();

            Assert.NotNull(body);
            Assert.Equal(playerId.Value, body.PlayerId);

            var item = Assert.Single(body.Items);
            Assert.Equal(itemId.Value, item.ItemId);
            Assert.Equal(7, item.Quantity);
        }
        finally
        {
            await factory.DisposeAsync();

            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }

    private sealed record GetInventoryResponseContract(
        Guid PlayerId,
        IReadOnlyCollection<InventoryItemContract> Items);

    private sealed record InventoryItemContract(
        Guid ItemId,
        int Quantity);
}
