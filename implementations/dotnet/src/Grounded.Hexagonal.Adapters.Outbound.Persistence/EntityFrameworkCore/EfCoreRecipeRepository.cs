using Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore.Mapping;
using Grounded.Hexagonal.Application.Ports.Outbound;
using Grounded.Hexagonal.Domain.Crafting;
using Microsoft.EntityFrameworkCore;

namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore;

/// <summary>
/// SQLite/EF Core implementation of PORT-OUT-RECIPE-001.
/// </summary>
public sealed class EfCoreRecipeRepository : IRecipeRepository
{
    private readonly GroundedDbContextFactory _dbContextFactory;

    public EfCoreRecipeRepository(
        SqlitePersistenceOptions persistenceOptions)
    {
        _dbContextFactory = new GroundedDbContextFactory(
            persistenceOptions);
    }

    public async Task<Recipe?> GetByIdAsync(
        RecipeId recipeId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(recipeId);

        await using var dbContext =
            _dbContextFactory.CreateDbContext();

        var record = await dbContext.Recipes
            .AsNoTracking()
            .Include(recipe => recipe.Ingredients)
            .SingleOrDefaultAsync(
                recipe => recipe.RecipeId == recipeId.Value,
                cancellationToken);

        return record is null
            ? null
            : DomainPersistenceMapper.ToDomain(record);
    }
}
