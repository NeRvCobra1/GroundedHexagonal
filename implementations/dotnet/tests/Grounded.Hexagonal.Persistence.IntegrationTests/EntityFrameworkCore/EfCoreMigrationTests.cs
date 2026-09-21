using Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace Grounded.Hexagonal.Persistence.IntegrationTests.EntityFrameworkCore;

public sealed class EfCoreMigrationTests
{
    private const string InitialMigrationId =
        "20260920220000_InitialCreate";

    [Fact]
    public async Task MigrateAsync_Should_Create_Expected_Schema_And_History()
    {
        await using var database =
            await SqliteTestDatabase.CreateAsync();

        await using var connection =
            new SqliteConnection(
                $"Data Source={database.DatabasePath};Pooling=False");

        await connection.OpenAsync();

        var tableNames =
            await ReadSingleColumnAsync(
                connection,
                """
                SELECT name
                FROM sqlite_master
                WHERE type = 'table'
                ORDER BY name;
                """);

        Assert.Contains("Foods", tableNames);
        Assert.Contains("Inventories", tableNames);
        Assert.Contains("InventoryItems", tableNames);
        Assert.Contains("Recipes", tableNames);
        Assert.Contains("RecipeIngredients", tableNames);
        Assert.Contains("__EFMigrationsHistory", tableNames);

        var migrations =
            await ReadSingleColumnAsync(
                connection,
                """
                SELECT MigrationId
                FROM __EFMigrationsHistory
                ORDER BY MigrationId;
                """);

        Assert.Contains(
            InitialMigrationId,
            migrations);
    }

    [Fact]
    public async Task MigrateAsync_Should_Be_Idempotent_When_Database_Is_Current()
    {
        await using var database =
            await SqliteTestDatabase.CreateAsync();

        var initializer =
            new EfCoreDatabaseInitializer(
                database.PersistenceOptions);

        await initializer.MigrateAsync();
        await initializer.MigrateAsync();

        await using var connection =
            new SqliteConnection(
                $"Data Source={database.DatabasePath};Pooling=False");

        await connection.OpenAsync();

        await using var command =
            connection.CreateCommand();

        command.CommandText =
            """
            SELECT COUNT(*)
            FROM __EFMigrationsHistory
            WHERE MigrationId = $migrationId;
            """;

        command.Parameters.AddWithValue(
            "$migrationId",
            InitialMigrationId);

        var count =
            Convert.ToInt32(
                await command.ExecuteScalarAsync());

        Assert.Equal(1, count);
    }

    private static async Task<string[]> ReadSingleColumnAsync(
        SqliteConnection connection,
        string commandText)
    {
        await using var command =
            connection.CreateCommand();

        command.CommandText = commandText;

        await using var reader =
            await command.ExecuteReaderAsync();

        var values = new List<string>();

        while (await reader.ReadAsync())
        {
            values.Add(reader.GetString(0));
        }

        return values.ToArray();
    }
}
