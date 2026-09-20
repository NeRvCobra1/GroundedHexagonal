using System.Collections.Concurrent;
using Grounded.Hexagonal.Application.Ports.Outbound;
using Grounded.Hexagonal.Domain.Food;

namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.InMemory;

/// <summary>
/// In-memory implementation of the food persistence outbound port.
/// </summary>
public sealed class InMemoryFoodRepository : IFoodRepository
{
    private readonly ConcurrentDictionary<FoodId, Food> _foods;

    public InMemoryFoodRepository()
        : this(Array.Empty<Food>())
    {
    }

    public InMemoryFoodRepository(IEnumerable<Food> foods)
    {
        ArgumentNullException.ThrowIfNull(foods);

        _foods = new ConcurrentDictionary<FoodId, Food>(
            foods.ToDictionary(
                food => food.Id,
                food => food));
    }

    public Task<IReadOnlyCollection<Food>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyCollection<Food> foods = _foods
            .Values
            .OrderBy(food => food.Id.Value)
            .ToArray();

        return Task.FromResult(foods);
    }

    public Task SaveAsync(
        Food food,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(food);
        cancellationToken.ThrowIfCancellationRequested();

        _foods[food.Id] = food;

        return Task.CompletedTask;
    }
}
