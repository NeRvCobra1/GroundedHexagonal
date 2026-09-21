using Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore;

namespace Grounded.Hexagonal.Persistence.IntegrationTests.EntityFrameworkCore;

internal sealed class SqliteTestDatabase : IAsyncDisposable
{
    private SqliteTestDatabase(
        string databasePath,
        SqlitePersistenceOptions persistenceOptions)
    {
        DatabasePath = databasePath;
        PersistenceOptions = persistenceOptions;
    }

    public string DatabasePath { get; }

    public SqlitePersistenceOptions PersistenceOptions { get; }

    public static async Task<SqliteTestDatabase> CreateAsync()
    {
        var databasePath = Path.Combine(
            Path.GetTempPath(),
            $"grounded-hexagonal-{Guid.NewGuid():N}.db");

        var persistenceOptions =
            new SqlitePersistenceOptions(
                $"Data Source={databasePath};Pooling=False");

        var database = new SqliteTestDatabase(
            databasePath,
            persistenceOptions);

        var initializer =
            new EfCoreDatabaseInitializer(
                persistenceOptions);

        await initializer.MigrateAsync();

        return database;
    }

    public ValueTask DisposeAsync()
    {
        if (File.Exists(DatabasePath))
        {
            File.Delete(DatabasePath);
        }

        return ValueTask.CompletedTask;
    }
}
