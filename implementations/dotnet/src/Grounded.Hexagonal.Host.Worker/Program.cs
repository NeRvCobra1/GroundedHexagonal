using Grounded.Hexagonal.Adapters.Inbound.Worker.FoodSpoilage;
using Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore;
using Grounded.Hexagonal.Adapters.Outbound.Time;
using Grounded.Hexagonal.Application.Ports.Inbound;
using Grounded.Hexagonal.Application.Ports.Outbound;
using Grounded.Hexagonal.Application.UseCases.ProcessFoodSpoilage;
using Grounded.Hexagonal.Host.Worker.Composition;

var builder = Host.CreateApplicationBuilder(args);

var sqlitePersistenceOptions =
    PersistenceComposition.AddConfiguredPersistence(
        builder.Services,
        builder.Configuration);

builder.Services.AddSingleton<IClock, SystemClock>();

builder.Services.AddTransient<
    IProcessFoodSpoilageUseCase,
    ProcessFoodSpoilageHandler>();

builder.Services.Configure<FoodSpoilageWorkerOptions>(
    builder.Configuration.GetSection("FoodSpoilage"));

builder.Services.AddHostedService<FoodSpoilageWorker>();

var host = builder.Build();

if (sqlitePersistenceOptions is not null)
{
    var initializer =
        new EfCoreDatabaseInitializer(
            sqlitePersistenceOptions);

    await initializer.MigrateAsync();
}

await host.RunAsync();
