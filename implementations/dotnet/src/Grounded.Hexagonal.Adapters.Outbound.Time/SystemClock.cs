using Grounded.Hexagonal.Application.Ports.Outbound;

namespace Grounded.Hexagonal.Adapters.Outbound.Time;

/// <summary>
/// System-time implementation of PORT-OUT-CLOCK-001.
/// </summary>
public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
