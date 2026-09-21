using Microsoft.EntityFrameworkCore.Design;

namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore;

/// <summary>
/// Creates the EF Core DbContext for design-time tooling such as
/// <c>dotnet ef migrations add</c>.
/// </summary>
/// <remarks>
/// This factory belongs to the persistence adapter. It exists only because
/// EF Core tooling needs a concrete way to construct the technology-specific
/// DbContext outside the normal Host composition flow.
/// </remarks>
public sealed class GroundedDbContextDesignTimeFactory
    : IDesignTimeDbContextFactory<GroundedDbContext>
{
    private const string DefaultConnectionString =
        "Data Source=grounded-hexagonal.design.db";

    public GroundedDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable(
                "GROUNDED_SQLITE_CONNECTION_STRING");

        var persistenceOptions =
            new SqlitePersistenceOptions(
                string.IsNullOrWhiteSpace(connectionString)
                    ? DefaultConnectionString
                    : connectionString);

        return new GroundedDbContextFactory(
            persistenceOptions)
            .CreateDbContext();
    }
}
