namespace Grounded.Hexagonal.Visualizer.Models;

public sealed class RepositoryMapSnapshot
{
    public IReadOnlyList<RepositoryNode> Areas { get; init; } = [];

    public IReadOnlyDictionary<string, RepositoryNode> NodesByPath { get; init; } =
        new Dictionary<string, RepositoryNode>(StringComparer.OrdinalIgnoreCase);

    public RepositoryMetrics Metrics { get; init; } = new();
}

public sealed class RepositoryNode
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string RelativePath { get; init; } = string.Empty;

    public string Kind { get; init; } = string.Empty;

    public string ArchitectureRole { get; init; } = string.Empty;

    public string? ParentPath { get; init; }

    public string? ProjectPath { get; init; }

    public string Extension { get; init; } = string.Empty;

    public int Depth { get; init; }

    public int FileCount { get; set; }

    public int LineCount { get; init; }

    public IReadOnlyList<RepositoryNode> Children { get; set; } = [];

    public IReadOnlyList<RepositoryCodeSymbol> Symbols { get; init; } = [];

    public bool IsFile => Kind.Equals("file", StringComparison.OrdinalIgnoreCase);

    public bool IsFolderLike => !IsFile;
}

public sealed class RepositoryCodeSymbol
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Kind { get; init; } = string.Empty;

    public string Signature { get; init; } = string.Empty;

    public int LineNumber { get; init; }
}

public sealed class RepositoryMetrics
{
    public int Areas { get; init; }

    public int Projects { get; init; }

    public int Folders { get; init; }

    public int Files { get; init; }

    public int CSharpFiles { get; init; }

    public int Symbols { get; init; }
}
