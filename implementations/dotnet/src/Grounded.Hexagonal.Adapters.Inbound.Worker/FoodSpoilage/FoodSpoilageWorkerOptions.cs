namespace Grounded.Hexagonal.Adapters.Inbound.Worker.FoodSpoilage;

/// <summary>
/// Scheduling configuration for the .NET worker adapter.
/// </summary>
/// <remarks>
/// This is adapter configuration, not a Domain concept.
/// </remarks>
public sealed class FoodSpoilageWorkerOptions
{
    public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(1);

    public bool RunImmediately { get; set; } = true;
}
