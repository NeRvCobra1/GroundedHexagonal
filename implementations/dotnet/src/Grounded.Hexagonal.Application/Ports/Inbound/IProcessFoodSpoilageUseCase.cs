using Grounded.Hexagonal.Application.UseCases.ProcessFoodSpoilage;

namespace Grounded.Hexagonal.Application.Ports.Inbound;

/// <summary>
/// Inbound port for the automatic food spoilage use case.
/// </summary>
/// <remarks>
/// Architecture ID: PORT-IN-SPOILAGE-001
/// Use Case: UC-SPOILAGE-001
/// </remarks>
public interface IProcessFoodSpoilageUseCase
{
    Task<ProcessFoodSpoilageResult> ExecuteAsync(
        ProcessFoodSpoilageCommand command,
        CancellationToken cancellationToken = default);
}
