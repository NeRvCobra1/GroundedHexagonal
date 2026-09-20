using Grounded.Hexagonal.Adapters.Outbound.Persistence.InMemory;
using Grounded.Hexagonal.Domain.Food;

namespace Grounded.Hexagonal.Persistence.IntegrationTests.InMemory;

public sealed class InMemoryFoodRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_Should_Return_Seeded_Food()
    {
        var first = new Domain.Food.Food(
            FoodId.New(),
            DateTimeOffset.UtcNow.AddMinutes(1));

        var second = new Domain.Food.Food(
            FoodId.New(),
            DateTimeOffset.UtcNow.AddMinutes(2));

        var repository = new InMemoryFoodRepository(
            [first, second]);

        var foods = await repository.GetAllAsync();

        Assert.Equal(2, foods.Count);
        Assert.Contains(first, foods);
        Assert.Contains(second, foods);
    }

    [Fact]
    public async Task SaveAsync_Should_Add_New_Food()
    {
        var food = new Domain.Food.Food(
            FoodId.New(),
            DateTimeOffset.UtcNow.AddMinutes(1));

        var repository = new InMemoryFoodRepository();

        await repository.SaveAsync(food);

        var foods = await repository.GetAllAsync();

        Assert.Same(food, Assert.Single(foods));
    }
}
