using Grounded.Hexagonal.Application.Ports.Outbound;
using Grounded.Hexagonal.Application.UseCases.ProcessFoodSpoilage;
using Grounded.Hexagonal.Domain.Food;

namespace Grounded.Hexagonal.Application.Tests.ProcessFoodSpoilage;

public sealed class ProcessFoodSpoilageHandlerTests
{
    [Fact]
    public async Task ExecuteAsync_Should_Return_Zero_Counts_When_No_Food_Exists()
    {
        var repository = new FakeFoodRepository([]);
        var clock = new FixedClock(
            new DateTimeOffset(2026, 9, 20, 12, 0, 0, TimeSpan.Zero));

        var handler = new ProcessFoodSpoilageHandler(
            repository,
            clock);

        var result = await handler.ExecuteAsync(
            new ProcessFoodSpoilageCommand());

        Assert.Equal(0, result.EvaluatedCount);
        Assert.Equal(0, result.SpoiledCount);
        Assert.Equal(0, repository.SaveCalls);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Save_Only_Food_That_Became_Spoiled()
    {
        var now = new DateTimeOffset(
            2026, 9, 20, 12, 0, 0, TimeSpan.Zero);

        var dueFood = new Domain.Food.Food(
            FoodId.New(),
            now);

        var freshFood = new Domain.Food.Food(
            FoodId.New(),
            now.AddMinutes(1));

        var repository = new FakeFoodRepository(
            [dueFood, freshFood]);

        var handler = new ProcessFoodSpoilageHandler(
            repository,
            new FixedClock(now));

        var result = await handler.ExecuteAsync(
            new ProcessFoodSpoilageCommand());

        Assert.Equal(2, result.EvaluatedCount);
        Assert.Equal(1, result.SpoiledCount);

        Assert.True(dueFood.IsSpoiled);
        Assert.False(freshFood.IsSpoiled);

        Assert.Equal(1, repository.SaveCalls);
        Assert.Same(dueFood, Assert.Single(repository.SavedFoods));
    }

    [Fact]
    public async Task ExecuteAsync_Should_Not_Save_Food_That_Was_Already_Spoiled()
    {
        var now = new DateTimeOffset(
            2026, 9, 20, 12, 0, 0, TimeSpan.Zero);

        var alreadySpoiled = new Domain.Food.Food(
            FoodId.New(),
            now.AddMinutes(-5));

        alreadySpoiled.AdvanceSpoilage(
            now.AddMinutes(-4));

        var repository = new FakeFoodRepository(
            [alreadySpoiled]);

        var handler = new ProcessFoodSpoilageHandler(
            repository,
            new FixedClock(now));

        var result = await handler.ExecuteAsync(
            new ProcessFoodSpoilageCommand());

        Assert.Equal(1, result.EvaluatedCount);
        Assert.Equal(0, result.SpoiledCount);
        Assert.Equal(0, repository.SaveCalls);
    }

    private sealed class FixedClock : IClock
    {
        public FixedClock(DateTimeOffset utcNow)
        {
            UtcNow = utcNow;
        }

        public DateTimeOffset UtcNow { get; }
    }

    private sealed class FakeFoodRepository : IFoodRepository
    {
        private readonly IReadOnlyCollection<Domain.Food.Food> _foods;

        public FakeFoodRepository(
            IReadOnlyCollection<Domain.Food.Food> foods)
        {
            _foods = foods;
        }

        public int SaveCalls { get; private set; }

        public List<Domain.Food.Food> SavedFoods { get; } = [];

        public Task<IReadOnlyCollection<Domain.Food.Food>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_foods);
        }

        public Task SaveAsync(
            Domain.Food.Food food,
            CancellationToken cancellationToken = default)
        {
            SaveCalls++;
            SavedFoods.Add(food);

            return Task.CompletedTask;
        }
    }
}
