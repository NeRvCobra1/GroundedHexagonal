using Microsoft.EntityFrameworkCore;

namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore;

/// <summary>
/// Creates the SQLite schema used by the educational EF Core adapter.
/// </summary>
/// <remarks>
/// This milestone intentionally uses EnsureCreated instead of migrations.
/// Schema migrations will be introduced separately so the concepts remain
/// visible one at a time.
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

    public async Task EnsureCreatedAsync(
        CancellationToken cancellationToken = default)
    {
        await using var dbContext =
            _dbContextFactory.CreateDbContext();

        await dbContext.Database.EnsureCreatedAsync(
            cancellationToken);
    }
}
