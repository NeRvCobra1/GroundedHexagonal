using Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore;
using Grounded.Hexagonal.Adapters.Outbound.Persistence.InMemory;
using Grounded.Hexagonal.Application.Ports.Outbound;
using Grounded.Hexagonal.Host.Worker.Composition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Grounded.Hexagonal.Worker.IntegrationTests.Composition;

public sealed class PersistenceCompositionTests
{
    [Fact]
    public void Worker_Host_Should_Default_To_InMemory_Persistence()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        var sqliteOptions =
            PersistenceComposition.AddConfiguredPersistence(
                services,
                configuration);

        using var provider = services.BuildServiceProvider();

        var repository =
            provider.GetRequiredService<IFoodRepository>();

        Assert.Null(sqliteOptions);
        Assert.IsType<InMemoryFoodRepository>(repository);
    }

    [Fact]
    public void Worker_Host_Should_Register_EfCore_Repository_When_Sqlite_Is_Configured()
    {
        var services = new ServiceCollection();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Persistence:Provider"] = "Sqlite",
                    ["ConnectionStrings:Grounded"] = "Data Source=test.db;Pooling=False"
                })
            .Build();

        var sqliteOptions =
            PersistenceComposition.AddConfiguredPersistence(
                services,
                configuration);

        using var provider = services.BuildServiceProvider();

        var repository =
            provider.GetRequiredService<IFoodRepository>();

        var options =
            Assert.IsType<SqlitePersistenceOptions>(sqliteOptions);

        Assert.Equal(
            "Data Source=test.db;Pooling=False",
            options.ConnectionString);
        Assert.IsType<EfCoreFoodRepository>(repository);
    }
}
