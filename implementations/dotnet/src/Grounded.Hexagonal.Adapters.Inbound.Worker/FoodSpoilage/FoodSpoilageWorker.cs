using Grounded.Hexagonal.Application.Ports.Inbound;
using Grounded.Hexagonal.Application.UseCases.ProcessFoodSpoilage;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Grounded.Hexagonal.Adapters.Inbound.Worker.FoodSpoilage;

/// <summary>
/// BackgroundService inbound adapter for UC-SPOILAGE-001.
/// </summary>
/// <remarks>
/// The worker owns scheduling concerns and invokes the inbound port.
/// It contains no spoilage rules and performs no persistence directly.
/// </remarks>
public sealed class FoodSpoilageWorker : BackgroundService
{
    private readonly IProcessFoodSpoilageUseCase _useCase;
    private readonly FoodSpoilageWorkerOptions _options;
    private readonly ILogger<FoodSpoilageWorker> _logger;

    public FoodSpoilageWorker(
        IProcessFoodSpoilageUseCase useCase,
        IOptions<FoodSpoilageWorkerOptions> options,
        ILogger<FoodSpoilageWorker> logger)
    {
        ArgumentNullException.ThrowIfNull(useCase);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        _useCase = useCase;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ProcessFoodSpoilageResult> RunOnceAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await _useCase.ExecuteAsync(
            new ProcessFoodSpoilageCommand(),
            cancellationToken);

        _logger.LogInformation(
            "Food spoilage cycle completed. Evaluated: {EvaluatedCount}; Spoiled: {SpoiledCount}.",
            result.EvaluatedCount,
            result.SpoiledCount);

        return result;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        if (_options.Interval <= TimeSpan.Zero)
        {
            throw new InvalidOperationException(
                "FoodSpoilage worker interval must be greater than zero.");
        }

        if (!_options.RunImmediately)
        {
            await Task.Delay(
                _options.Interval,
                stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await RunOnceAsync(stoppingToken);

            await Task.Delay(
                _options.Interval,
                stoppingToken);
        }
    }
}
