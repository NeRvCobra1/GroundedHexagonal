using Grounded.Hexagonal.Adapters.Inbound.Http.CraftItem;
using Grounded.Hexagonal.Adapters.Inbound.Http.GetInventory;
using Grounded.Hexagonal.Adapters.Outbound.Persistence.InMemory;
using Grounded.Hexagonal.Application.Ports.Inbound;
using Grounded.Hexagonal.Application.Ports.Outbound;
using Grounded.Hexagonal.Application.UseCases.CraftItem;
using Grounded.Hexagonal.Application.UseCases.GetInventory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddSingleton<IInventoryRepository, InMemoryInventoryRepository>();
builder.Services.AddSingleton<IRecipeRepository, InMemoryRecipeRepository>();

builder.Services.AddTransient<ICraftItemUseCase, CraftItemHandler>();
builder.Services.AddTransient<IGetInventoryUseCase, GetInventoryHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapCraftItemEndpoint();
app.MapGetInventoryEndpoint();

app.Run();

public partial class Program
{
}
