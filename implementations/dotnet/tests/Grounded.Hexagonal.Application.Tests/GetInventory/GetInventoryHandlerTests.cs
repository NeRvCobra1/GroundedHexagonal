using Grounded.Hexagonal.Application.Ports.Outbound;
using Grounded.Hexagonal.Application.UseCases.GetInventory;
using Grounded.Hexagonal.Domain.Inventory;
using Grounded.Hexagonal.Domain.Items;
using InventoryAggregate = Grounded.Hexagonal.Domain.Inventory.Inventory;

namespace Grounded.Hexagonal.Application.Tests.GetInventory;

public sealed class GetInventoryHandlerTests
{
    [Fact]
    public void Query_Should_Reject_Null_PlayerId()
    {
        Assert.Throws<ArgumentNullException>(
            () => new GetInventoryQuery(null!));
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_InventoryNotFound_When_Inventory_Does_Not_Exist()
    {
        var playerId = PlayerId.New();
        var repository = new FakeInventoryRepository(null);
        var handler = new GetInventoryHandler(repository);

        var result = await handler.ExecuteAsync(
            new GetInventoryQuery(playerId));

        Assert.Equal(
            GetInventoryStatus.InventoryNotFound,
            result.Status);

        Assert.False(result.IsSuccess);
        Assert.Empty(result.Items);
        Assert.Equal(1, repository.GetCalls);
        Assert.Equal(0, repository.SaveCalls);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Current_Inventory_Without_Saving()
    {
        var playerId = PlayerId.New();
        var firstItem = ItemId.New();
        var secondItem = ItemId.New();

        var inventory = new InventoryAggregate(playerId);
        inventory.Add(firstItem, 4);
        inventory.Add(secondItem, 7);

        var repository = new FakeInventoryRepository(inventory);
        var handler = new GetInventoryHandler(repository);

        var result = await handler.ExecuteAsync(
            new GetInventoryQuery(playerId));

        Assert.Equal(
            GetInventoryStatus.Success,
            result.Status);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Items.Count);

        Assert.Contains(
            result.Items,
            item => item.ItemId == firstItem && item.Quantity == 4);

        Assert.Contains(
            result.Items,
            item => item.ItemId == secondItem && item.Quantity == 7);

        Assert.Equal(1, repository.GetCalls);
        Assert.Equal(0, repository.SaveCalls);
    }

    private sealed class FakeInventoryRepository : IInventoryRepository
    {
        private readonly InventoryAggregate? _inventory;

        public FakeInventoryRepository(
            InventoryAggregate? inventory)
        {
            _inventory = inventory;
        }

        public int GetCalls { get; private set; }

        public int SaveCalls { get; private set; }

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
            return Task.CompletedTask;
        }
    }
}
