using Grounded.Hexagonal.Application.Ports.Inbound;
using Grounded.Hexagonal.Application.Ports.Outbound;

namespace Grounded.Hexagonal.Application.UseCases.ProcessFoodSpoilage;

/// <summary>
/// Coordinates UC-SPOILAGE-001.
/// </summary>
/// <remarks>
/// The handler obtains the current time and persisted food through outbound
/// ports, delegates the spoilage rule to Domain and persists only food whose
/// state actually changed.
/// </remarks>
public sealed class ProcessFoodSpoilageHandler : IProcessFoodSpoilageUseCase
{
    private readonly IFoodRepository _foodRepository;
    private readonly IClock _clock;

    public ProcessFoodSpoilageHandler(
        IFoodRepository foodRepository,
        IClock clock)
    {
        ArgumentNullException.ThrowIfNull(foodRepository);
        ArgumentNullException.ThrowIfNull(clock);

        _foodRepository = foodRepository;
        _clock = clock;
    }

    public async Task<ProcessFoodSpoilageResult> ExecuteAsync(
        ProcessFoodSpoilageCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var currentTime = _clock.UtcNow;
        var foods = await _foodRepository.GetAllAsync(cancellationToken);

        var spoiledCount = 0;

        foreach (var food in foods)
        {
            if (!food.AdvanceSpoilage(currentTime))
            {
                continue;
            }

            await _foodRepository.SaveAsync(
                food,
                cancellationToken);

            spoiledCount++;
        }

        return new ProcessFoodSpoilageResult(
            EvaluatedCount: foods.Count,
            SpoiledCount: spoiledCount);
    }
}
