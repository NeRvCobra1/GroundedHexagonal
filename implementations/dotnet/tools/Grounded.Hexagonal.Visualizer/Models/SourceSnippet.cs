namespace Grounded.Hexagonal.Visualizer.Models;

public sealed class SourceSnippet
{
    public static SourceSnippet Empty(string message) =>
        new()
        {
            IsAvailable = false,
            Message = message
        };

    public bool IsAvailable { get; init; }

    public string Message { get; init; } = string.Empty;

    public string FilePath { get; init; } = string.Empty;

    public IReadOnlyList<SourceSnippetLine> Lines { get; init; } = [];
}

public sealed record SourceSnippetLine(
    int Number,
    string Text);
