using Grounded.Hexagonal.Visualizer.Components;
using Grounded.Hexagonal.Visualizer.Services;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

// This visualizer is a local development tool. `dotnet run` can start it in
// Production when no launch profile is present, but Blazor framework assets
// still need the static-web-assets manifest while running from build output.
// Enabling it explicitly makes the tool self-contained in both Development
// and local Production runs.
builder.WebHost.UseStaticWebAssets();

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<ScenarioCatalog>();
builder.Services.AddSingleton<SourceSnippetService>();
builder.Services.AddSingleton<RepositoryMapService>();

var app = builder.Build();

// Serve physical wwwroot files such as app.css and also map the static web
// assets manifest that contains the Blazor framework resources.
app.UseStaticFiles();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
