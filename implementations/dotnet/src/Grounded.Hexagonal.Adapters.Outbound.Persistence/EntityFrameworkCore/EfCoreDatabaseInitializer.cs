using Microsoft.EntityFrameworkCore;

namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore;

/// <summary>
/// Applies pending EF Core migrations for the SQLite persistence adapter.
/// </summary>
/// <remarks>
/// Runtime migration is convenient for this educational reference project.
/// Production systems may prefer reviewed SQL scripts or migration bundles,
/// depending on deployment and database-permission requirements.
/// </remarks>
public sealed class EfCoreDatabaseInitializer
{
    private readonly GroundedDbContextFactory _dbContextFactory;

    public EfCoreDatabaseInitializer(
        SqlitePersistenceOptions persistenceOptions)
    {
        _dbContextFactory = new GroundedDbContextFactory(
            persistenceOptions);
    }

    public async Task MigrateAsync(
        CancellationToken cancellationToken = default)
    {
        await using var dbContext =
            _dbContextFactory.CreateDbContext();

        await dbContext.Database.MigrateAsync(
            cancellationToken);
    }
}
