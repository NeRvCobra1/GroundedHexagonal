namespace Grounded.Hexagonal.Visualizer.Models;

public sealed class ArchitectureScenario
{
    public string Id { get; init; } = string.Empty;

    public string UseCaseId { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string PlaybackNote { get; init; } = string.Empty;

    public IReadOnlyList<FlowStep> Steps { get; init; } = [];

    public IReadOnlyList<ProjectNode> Projects { get; init; } = [];

    public IReadOnlyList<ProjectEdge> ProjectEdges { get; init; } = [];
}

public sealed class FlowStep
{
    public string Id { get; init; } = string.Empty;

    public int Order { get; init; }

    public string Title { get; init; } = string.Empty;

    public string ShortLabel { get; init; } = string.Empty;

    public string GeneralStage { get; init; } = string.Empty;

    public string HexRole { get; init; } = string.Empty;

    public string Implementation { get; init; } = string.Empty;

    public string Explanation { get; init; } = string.Empty;

    public string ArchitectureId { get; init; } = string.Empty;

    public string DirectionNote { get; init; } = string.Empty;

    public FlowPosition Position { get; init; } = new();

    public PayloadState Payload { get; init; } = new();

    public CodeReference? Code { get; init; }
}

public sealed class FlowPosition
{
    public double X { get; init; }

    public double Y { get; init; }
}

public sealed class PayloadState
{
    public string Label { get; init; } = string.Empty;

    public string Type { get; init; } = string.Empty;

    public string Preview { get; init; } = string.Empty;
}

public sealed class CodeReference
{
    public string FilePath { get; init; } = string.Empty;

    public string StartContains { get; init; } = string.Empty;

    public int TakeLines { get; init; } = 20;
}

public sealed class ProjectNode
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string ShortName { get; init; } = string.Empty;

    public string PathPrefix { get; init; } = string.Empty;

    public string Responsibility { get; init; } = string.Empty;

    public double X { get; init; }

    public double Y { get; init; }
}

public sealed class ProjectEdge
{
    public string From { get; init; } = string.Empty;

    public string To { get; init; } = string.Empty;
}
