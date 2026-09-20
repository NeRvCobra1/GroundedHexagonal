using Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore.Mapping;
using Grounded.Hexagonal.Domain.Crafting;
using Grounded.Hexagonal.Domain.Food;
using Grounded.Hexagonal.Domain.Inventory;
using Microsoft.EntityFrameworkCore;
using InventoryAggregate = Grounded.Hexagonal.Domain.Inventory.Inventory;

namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore;

/// <summary>
/// Adapter-specific helper used to seed concrete SQLite persistence.
/// </summary>
/// <remarks>
/// This is not an Application port. It exists outside the Core because
/// seeding a particular persistence technology is an infrastructure concern.
/// </remarks>
public sealed class EfCoreDataSeeder
{
    private readonly GroundedDbContextFactory _dbContextFactory;
    private readonly EfCoreInventoryRepository _inventoryRepository;
    private readonly EfCoreFoodRepository _foodRepository;

    public EfCoreDataSeeder(
        SqlitePersistenceOptions persistenceOptions)
    {
        _dbContextFactory = new GroundedDbContextFactory(
            persistenceOptions);

        _inventoryRepository = new EfCoreInventoryRepository(
            persistenceOptions);

        _foodRepository = new EfCoreFoodRepository(
            persistenceOptions);
    }

    public Task SeedInventoryAsync(
        InventoryAggregate inventory,
        CancellationToken cancellationToken = default)
    {
        return _inventoryRepository.SaveAsync(
            inventory,
            cancellationToken);
    }

    public async Task SeedRecipeAsync(
        Recipe recipe,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(recipe);

        await using var dbContext =
            _dbContextFactory.CreateDbContext();

        var existing = await dbContext.Recipes
            .Include(record => record.Ingredients)
            .SingleOrDefaultAsync(
                record => record.RecipeId == recipe.Id.Value,
                cancellationToken);

        if (existing is not null)
        {
            dbContext.Recipes.Remove(existing);

            await dbContext.SaveChangesAsync(
                cancellationToken);
        }

        dbContext.Recipes.Add(
            DomainPersistenceMapper.ToRecord(recipe));

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public Task SeedFoodAsync(
        Food food,
        CancellationToken cancellationToken = default)
    {
        return _foodRepository.SaveAsync(
            food,
            cancellationToken);
    }
}
