using Microsoft.EntityFrameworkCore;

namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore;

internal sealed class GroundedDbContextFactory
{
    private readonly DbContextOptions<GroundedDbContext> _dbContextOptions;

    public GroundedDbContextFactory(
        SqlitePersistenceOptions persistenceOptions)
    {
        ArgumentNullException.ThrowIfNull(persistenceOptions);

        _dbContextOptions =
            new DbContextOptionsBuilder<GroundedDbContext>()
                .UseSqlite(persistenceOptions.ConnectionString)
                .Options;
    }

    public GroundedDbContext CreateDbContext() =>
        new(_dbContextOptions);
}
