using Grounded.Hexagonal.Domain.Food;

namespace Grounded.Hexagonal.Application.Ports.Outbound;

/// <summary>
/// Outbound port used to load and persist food instances.
/// </summary>
/// <remarks>
/// Supporting architecture ID: PORT-OUT-FOOD-001
///
/// The contract expresses the capability required by Application without
/// specifying database, ORM or storage technology.
/// </remarks>
public interface IFoodRepository
{
    Task<IReadOnlyCollection<Food>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        Food food,
        CancellationToken cancellationToken = default);
}
