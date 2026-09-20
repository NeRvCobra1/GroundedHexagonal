namespace Grounded.Hexagonal.Application.UseCases.ProcessFoodSpoilage;

/// <summary>
/// Application result for one execution of UC-SPOILAGE-001.
/// </summary>
public sealed record ProcessFoodSpoilageResult(
    int EvaluatedCount,
    int SpoiledCount);
