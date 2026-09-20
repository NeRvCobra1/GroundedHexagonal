namespace Grounded.Hexagonal.Application.Ports.Outbound;

/// <summary>
/// Outbound port for obtaining the current time.
/// </summary>
/// <remarks>
/// Architecture ID: PORT-OUT-CLOCK-001
///
/// Application owns the abstraction so the use case does not depend directly
/// on the system clock.
/// </remarks>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
