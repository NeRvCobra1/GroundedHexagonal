using System.Text.Json;
using Grounded.Hexagonal.Visualizer.Models;

namespace Grounded.Hexagonal.Visualizer.Services;

public sealed class ScenarioCatalog
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IWebHostEnvironment _environment;
    private IReadOnlyList<ArchitectureScenario>? _cachedScenarios;

    public ScenarioCatalog(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<IReadOnlyList<ArchitectureScenario>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        if (_cachedScenarios is not null)
        {
            return _cachedScenarios;
        }

        var scenarioDirectory = Path.Combine(
            _environment.ContentRootPath,
            "Data",
            "Scenarios");

        if (!Directory.Exists(scenarioDirectory))
        {
            return [];
        }

        var scenarios = new List<ArchitectureScenario>();

        foreach (var filePath in Directory
                     .EnumerateFiles(scenarioDirectory, "*.json")
                     .OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
        {
            await using var stream = File.OpenRead(filePath);
            var scenario = await JsonSerializer.DeserializeAsync<ArchitectureScenario>(
                stream,
                SerializerOptions,
                cancellationToken);

            if (scenario is not null)
            {
                scenarios.Add(scenario);
            }
        }

        _cachedScenarios = scenarios;
        return _cachedScenarios;
    }
}
