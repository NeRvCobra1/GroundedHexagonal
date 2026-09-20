using System.Net;
using System.Net.Http.Json;
using Grounded.Hexagonal.Adapters.Outbound.Persistence.InMemory;
using Grounded.Hexagonal.Application.Ports.Outbound;
using Grounded.Hexagonal.Domain.Inventory;
using Grounded.Hexagonal.Domain.Items;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using InventoryAggregate = Grounded.Hexagonal.Domain.Inventory.Inventory;

namespace Grounded.Hexagonal.Http.IntegrationTests.GetInventory;

public sealed class GetInventoryHttpTests
{
    [Fact]
    public async Task Get_Should_Return_200_With_Current_Inventory()
    {
        var playerId = PlayerId.New();
        var firstItem = ItemId.New();
        var secondItem = ItemId.New();

        var inventory = new InventoryAggregate(playerId);
        inventory.Add(firstItem, 4);
        inventory.Add(secondItem, 7);

        await using var factory = CreateFactory(
            new InMemoryInventoryRepository([inventory]));

        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            $"/api/inventories/{playerId.Value}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<GetInventoryResponseContract>();

        Assert.NotNull(body);
        Assert.Equal(playerId.Value, body.PlayerId);
        Assert.Equal(2, body.Items.Count);

        Assert.Contains(
            body.Items,
            item => item.ItemId == firstItem.Value && item.Quantity == 4);

        Assert.Contains(
            body.Items,
            item => item.ItemId == secondItem.Value && item.Quantity == 7);
    }

    [Fact]
    public async Task Get_Should_Return_200_With_Empty_Items_When_Inventory_Exists_But_Is_Empty()
    {
        var playerId = PlayerId.New();
        var inventory = new InventoryAggregate(playerId);

        await using var factory = CreateFactory(
            new InMemoryInventoryRepository([inventory]));

        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            $"/api/inventories/{playerId.Value}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<GetInventoryResponseContract>();

        Assert.NotNull(body);
        Assert.Equal(playerId.Value, body.PlayerId);
        Assert.Empty(body.Items);
    }

    [Fact]
    public async Task Get_Should_Return_404_When_Inventory_Does_Not_Exist()
    {
        var playerId = PlayerId.New();

        await using var factory = CreateFactory(
            new InMemoryInventoryRepository());

        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            $"/api/inventories/{playerId.Value}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ErrorResponseContract>();

        Assert.NotNull(body);
        Assert.Equal("InventoryNotFound", body.Status);
    }

    [Fact]
    public async Task Get_Should_Return_400_When_PlayerId_Is_Empty()
    {
        await using var factory = CreateFactory(
            new InMemoryInventoryRepository());

        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            $"/api/inventories/{Guid.Empty}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ErrorResponseContract>();

        Assert.NotNull(body);
        Assert.Equal("InvalidPlayerId", body.Status);
    }

    private static WebApplicationFactory<Program> CreateFactory(
        InMemoryInventoryRepository inventoryRepository)
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");

                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IInventoryRepository>();

                    services.AddSingleton<IInventoryRepository>(
                        inventoryRepository);
                });
            });
    }

    private sealed record GetInventoryResponseContract(
        Guid PlayerId,
        IReadOnlyCollection<InventoryItemContract> Items);

    private sealed record InventoryItemContract(
        Guid ItemId,
        int Quantity);

    private sealed record ErrorResponseContract(
        string Status);
}
