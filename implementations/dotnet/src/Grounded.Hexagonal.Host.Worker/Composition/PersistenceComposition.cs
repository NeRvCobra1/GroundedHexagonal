using Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore;
using Grounded.Hexagonal.Adapters.Outbound.Persistence.InMemory;
using Grounded.Hexagonal.Application.Ports.Outbound;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Grounded.Hexagonal.Host.Worker.Composition;

/// <summary>
/// Selects the concrete persistence adapter used by the Worker host.
/// </summary>
/// <remarks>
/// The Worker only composes the persistence capability required by its
/// use case: IFoodRepository.
/// </remarks>
public static class PersistenceComposition
{
    public const string ProviderConfigurationKey = "Persistence:Provider";
    public const string ConnectionStringName = "Grounded";

    public static SqlitePersistenceOptions? AddConfiguredPersistence(
        IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var provider = configuration[ProviderConfigurationKey];

        if (string.IsNullOrWhiteSpace(provider) ||
            provider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IFoodRepository, InMemoryFoodRepository>();

            return null;
        }

        if (provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
        {
            var connectionString =
                configuration.GetConnectionString(ConnectionStringName);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    $"Persistence provider 'Sqlite' requires " +
                    $"ConnectionStrings:{ConnectionStringName}.");
            }

            var persistenceOptions =
                new SqlitePersistenceOptions(connectionString);

            services.AddSingleton(persistenceOptions);
            services.AddSingleton<IFoodRepository, EfCoreFoodRepository>();

            return persistenceOptions;
        }

        throw new InvalidOperationException(
            $"Unsupported persistence provider '{provider}'. " +
            "Supported values are 'InMemory' and 'Sqlite'.");
    }
}
