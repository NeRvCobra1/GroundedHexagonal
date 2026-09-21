using System.Net;
using System.Net.Http.Json;
using Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore.DemoData;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Grounded.Hexagonal.Http.IntegrationTests.Composition;

public sealed class DemoDataPersistenceHttpTests
{
    [Fact]
    public async Task Demo_Data_Should_Persist_After_Host_Restart_Without_Being_Reset()
    {
        var databasePath = Path.Combine(
            Path.GetTempPath(),
            $"grounded-demo-{Guid.NewGuid():N}.db");

        var connectionString =
            $"Data Source={databasePath};Pooling=False";

        try
        {
            await using (var firstFactory =
                CreateSqliteDemoFactory(connectionString))
            {
                using var firstClient =
                    firstFactory.CreateClient();

                var initialInventory =
                    await GetInventoryAsync(firstClient);

                AssertQuantity(
                    initialInventory,
                    EfCoreDemoDataIds.MintShardItemId,
                    12);

                AssertQuantity(
                    initialInventory,
                    EfCoreDemoDataIds.ToughGunkItemId,
                    5);

                AssertQuantity(
                    initialInventory,
                    EfCoreDemoDataIds.FlowerPetalItemId,
                    8);

                Assert.DoesNotContain(
                    initialInventory.Items,
                    item => item.ItemId == EfCoreDemoDataIds.MintMaceItemId);

                var craftResponse =
                    await firstClient.PostAsJsonAsync(
                        "/api/crafting/items",
                        new
                        {
                            playerId = EfCoreDemoDataIds.PlayerId,
                            recipeId = EfCoreDemoDataIds.MintMaceRecipeId
                        });

                Assert.Equal(
                    HttpStatusCode.OK,
                    craftResponse.StatusCode);

                var craftedInventory =
                    await GetInventoryAsync(firstClient);

                AssertQuantity(
                    craftedInventory,
                    EfCoreDemoDataIds.MintShardItemId,
                    2);

                Assert.DoesNotContain(
                    craftedInventory.Items,
                    item => item.ItemId == EfCoreDemoDataIds.ToughGunkItemId);

                AssertQuantity(
                    craftedInventory,
                    EfCoreDemoDataIds.FlowerPetalItemId,
                    5);

                AssertQuantity(
                    craftedInventory,
                    EfCoreDemoDataIds.MintMaceItemId,
                    1);
            }

            await using (var restartedFactory =
                CreateSqliteDemoFactory(connectionString))
            {
                using var restartedClient =
                    restartedFactory.CreateClient();

                var persistedInventory =
                    await GetInventoryAsync(restartedClient);

                AssertQuantity(
                    persistedInventory,
                    EfCoreDemoDataIds.MintShardItemId,
                    2);

                Assert.DoesNotContain(
                    persistedInventory.Items,
                    item => item.ItemId == EfCoreDemoDataIds.ToughGunkItemId);

                AssertQuantity(
                    persistedInventory,
                    EfCoreDemoDataIds.FlowerPetalItemId,
                    5);

                AssertQuantity(
                    persistedInventory,
                    EfCoreDemoDataIds.MintMaceItemId,
                    1);
            }
        }
        finally
        {
            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }

    private static WebApplicationFactory<Program> CreateSqliteDemoFactory(
        string connectionString)
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.UseSetting(
                    "Persistence:Provider",
                    "Sqlite");
                builder.UseSetting(
                    "ConnectionStrings:Grounded",
                    connectionString);
                builder.UseSetting(
                    "DemoData:Seed",
                    "true");
            });
    }

    private static async Task<GetInventoryResponseContract> GetInventoryAsync(
        HttpClient client)
    {
        var response = await client.GetAsync(
            $"/api/inventories/{EfCoreDemoDataIds.PlayerId}");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var body =
            await response.Content.ReadFromJsonAsync<GetInventoryResponseContract>();

        return Assert.IsType<GetInventoryResponseContract>(body);
    }

    private static void AssertQuantity(
        GetInventoryResponseContract inventory,
        Guid itemId,
        int expectedQuantity)
    {
        var item = Assert.Single(
            inventory.Items,
            candidate => candidate.ItemId == itemId);

        Assert.Equal(
            expectedQuantity,
            item.Quantity);
    }

    private sealed record GetInventoryResponseContract(
        Guid PlayerId,
        IReadOnlyCollection<InventoryItemContract> Items);

    private sealed record InventoryItemContract(
        Guid ItemId,
        int Quantity);
}
