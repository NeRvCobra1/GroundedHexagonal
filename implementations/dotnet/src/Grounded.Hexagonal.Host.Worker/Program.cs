using Grounded.Hexagonal.Adapters.Inbound.Worker.FoodSpoilage;
using Grounded.Hexagonal.Adapters.Outbound.Persistence.InMemory;
using Grounded.Hexagonal.Adapters.Outbound.Time;
using Grounded.Hexagonal.Application.Ports.Inbound;
using Grounded.Hexagonal.Application.Ports.Outbound;
using Grounded.Hexagonal.Application.UseCases.ProcessFoodSpoilage;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IFoodRepository, InMemoryFoodRepository>();
builder.Services.AddSingleton<IClock, SystemClock>();

builder.Services.AddTransient<
    IProcessFoodSpoilageUseCase,
    ProcessFoodSpoilageHandler>();

builder.Services.Configure<FoodSpoilageWorkerOptions>(
    builder.Configuration.GetSection("FoodSpoilage"));

builder.Services.AddHostedService<FoodSpoilageWorker>();

var host = builder.Build();
host.Run();
