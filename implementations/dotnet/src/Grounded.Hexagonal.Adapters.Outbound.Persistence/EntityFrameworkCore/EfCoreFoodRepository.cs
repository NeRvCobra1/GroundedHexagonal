using Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore.Mapping;
using Grounded.Hexagonal.Application.Ports.Outbound;
using Grounded.Hexagonal.Domain.Food;
using Microsoft.EntityFrameworkCore;

namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore;

/// <summary>
/// SQLite/EF Core implementation of PORT-OUT-FOOD-001.
/// </summary>
public sealed class EfCoreFoodRepository : IFoodRepository
{
    private readonly GroundedDbContextFactory _dbContextFactory;

    public EfCoreFoodRepository(
        SqlitePersistenceOptions persistenceOptions)
    {
        _dbContextFactory = new GroundedDbContextFactory(
            persistenceOptions);
    }

    public async Task<IReadOnlyCollection<Food>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var dbContext =
            _dbContextFactory.CreateDbContext();

        var records = await dbContext.Foods
            .AsNoTracking()
            .OrderBy(food => food.FoodId)
            .ToArrayAsync(cancellationToken);

        return records
            .Select(DomainPersistenceMapper.ToDomain)
            .ToArray();
    }

    public async Task SaveAsync(
        Food food,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(food);

        await using var dbContext =
            _dbContextFactory.CreateDbContext();

        var record = await dbContext.Foods
            .SingleOrDefaultAsync(
                existing => existing.FoodId == food.Id.Value,
                cancellationToken);

        if (record is null)
        {
            dbContext.Foods.Add(
                DomainPersistenceMapper.ToRecord(food));
        }
        else
        {
            record.SpoilsAtUtc = food.SpoilsAt.UtcDateTime;
            record.State = (int)food.State;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
