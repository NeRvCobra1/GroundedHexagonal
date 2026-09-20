namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore;

/// <summary>
/// Technology-specific configuration for the SQLite persistence adapter.
/// </summary>
public sealed record SqlitePersistenceOptions
{
    public SqlitePersistenceOptions(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException(
                "A SQLite connection string is required.",
                nameof(connectionString));
        }

        ConnectionString = connectionString;
    }

    public string ConnectionString { get; }
}
