namespace Grounded.Hexagonal.Application.UseCases.ProcessFoodSpoilage;

/// <summary>
/// Input model for UC-SPOILAGE-001.
/// </summary>
/// <remarks>
/// The automatic process currently requires no caller-provided data.
/// The explicit command still makes the inbound use-case boundary visible.
/// </remarks>
public sealed record ProcessFoodSpoilageCommand
{
}
