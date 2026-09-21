using Grounded.Hexagonal.Adapters.Inbound.Http.CraftItem;
using Grounded.Hexagonal.Adapters.Inbound.Http.GetInventory;
using Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore;
using Grounded.Hexagonal.Application.Ports.Inbound;
using Grounded.Hexagonal.Application.UseCases.CraftItem;
using Grounded.Hexagonal.Application.UseCases.GetInventory;
using Grounded.Hexagonal.Host.Api.Composition;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var sqlitePersistenceOptions =
    PersistenceComposition.AddConfiguredPersistence(
        builder.Services,
        builder.Configuration);

builder.Services.AddTransient<ICraftItemUseCase, CraftItemHandler>();
builder.Services.AddTransient<IGetInventoryUseCase, GetInventoryHandler>();

var app = builder.Build();

if (sqlitePersistenceOptions is not null)
{
    var initializer =
        new EfCoreDatabaseInitializer(
            sqlitePersistenceOptions);

    await initializer.EnsureCreatedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapCraftItemEndpoint();
app.MapGetInventoryEndpoint();

await app.RunAsync();

public partial class Program
{
}
