using System.Text.RegularExpressions;
using Grounded.Hexagonal.Visualizer.Models;

namespace Grounded.Hexagonal.Visualizer.Services;

public sealed partial class RepositoryMapService
{
    private static readonly string[] RootAreas = ["src", "tests", "docs", "tools"];

    private static readonly HashSet<string> IgnoredDirectories = new(StringComparer.OrdinalIgnoreCase)
    {
        "bin",
        "obj",
        ".git",
        ".vs",
        "node_modules"
    };

    private static readonly HashSet<string> IncludedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".cs",
        ".csproj",
        ".json",
        ".md",
        ".razor",
        ".css",
        ".js",
        ".slnx",
        ".props"
    };

    private readonly string _dotnetRoot;
    private RepositoryMapSnapshot? _cachedSnapshot;

    public RepositoryMapService(IWebHostEnvironment environment)
    {
        _dotnetRoot = Path.GetFullPath(
            Path.Combine(
                environment.ContentRootPath,
                "..",
                ".."));
    }

    public async Task<RepositoryMapSnapshot> GetSnapshotAsync(
        CancellationToken cancellationToken = default)
    {
        if (_cachedSnapshot is not null)
        {
            return _cachedSnapshot;
        }

        var nodesByPath = new Dictionary<string, RepositoryNode>(StringComparer.OrdinalIgnoreCase);
        var areas = new List<RepositoryNode>();

        foreach (var areaName in RootAreas)
        {
            var fullPath = Path.Combine(_dotnetRoot, areaName);

            if (!Directory.Exists(fullPath))
            {
                continue;
            }

            var area = await BuildDirectoryNodeAsync(
                fullPath,
                areaName,
                parentPath: null,
                projectPath: null,
                depth: 0,
                isArea: true,
                nodesByPath,
                cancellationToken);

            areas.Add(area);
        }

        var rootFiles = Directory
            .EnumerateFiles(_dotnetRoot)
            .Where(IsIncludedFile)
            .OrderBy(path => Path.GetFileName(path) ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var configPath = Path.Combine(_dotnetRoot, ".config");
        var rootChildren = new List<RepositoryNode>();

        foreach (var filePath in rootFiles)
        {
            rootChildren.Add(await BuildFileNodeAsync(
                filePath,
                parentPath: "dotnet-root",
                projectPath: null,
                depth: 1,
                nodesByPath,
                cancellationToken));
        }

        if (Directory.Exists(configPath))
        {
            rootChildren.Add(await BuildDirectoryNodeAsync(
                configPath,
                ".config",
                parentPath: "dotnet-root",
                projectPath: null,
                depth: 1,
                isArea: false,
                nodesByPath,
                cancellationToken));
        }

        if (rootChildren.Count > 0)
        {
            var rootArea = new RepositoryNode
            {
                Id = "area-dotnet-root",
                Name = "dotnet root",
                RelativePath = "dotnet-root",
                Kind = "area",
                ArchitectureRole = "Solution / configuration",
                Depth = 0,
                Children = rootChildren
            };

            rootArea.FileCount = CountFiles(rootArea);
            nodesByPath[rootArea.RelativePath] = rootArea;
            areas.Insert(0, rootArea);
        }

        _cachedSnapshot = new RepositoryMapSnapshot
        {
            Areas = areas,
            NodesByPath = nodesByPath,
            Metrics = BuildMetrics(areas)
        };

        return _cachedSnapshot;
    }

    public RepositoryNode? FindNode(
        RepositoryMapSnapshot snapshot,
        string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return null;
        }

        var normalized = NormalizeRelativePath(relativePath);

        return snapshot.NodesByPath.TryGetValue(normalized, out var node)
            ? node
            : null;
    }

    public IReadOnlyList<RepositoryNode> GetLineage(
        RepositoryMapSnapshot snapshot,
        RepositoryNode? node)
    {
        if (node is null)
        {
            return [];
        }

        var lineage = new List<RepositoryNode>();
        var current = node;

        while (true)
        {
            lineage.Add(current);

            if (string.IsNullOrWhiteSpace(current.ParentPath) ||
                !snapshot.NodesByPath.TryGetValue(current.ParentPath, out var parent))
            {
                break;
            }

            current = parent;
        }

        lineage.Reverse();
        return lineage;
    }

    public IReadOnlyList<RepositoryNode> Search(
        RepositoryMapSnapshot snapshot,
        string query,
        int limit = 80)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        var trimmed = query.Trim();

        return snapshot.NodesByPath.Values
            .Where(node =>
                node.Name.Contains(trimmed, StringComparison.OrdinalIgnoreCase) ||
                node.RelativePath.Contains(trimmed, StringComparison.OrdinalIgnoreCase) ||
                node.Symbols.Any(symbol =>
                    symbol.Name.Contains(trimmed, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(node => GetSearchRank(node, trimmed))
            .ThenBy(node => node.RelativePath, StringComparer.OrdinalIgnoreCase)
            .Take(Math.Clamp(limit, 1, 200))
            .ToArray();
    }

    private async Task<RepositoryNode> BuildDirectoryNodeAsync(
        string fullPath,
        string relativePath,
        string? parentPath,
        string? projectPath,
        int depth,
        bool isArea,
        IDictionary<string, RepositoryNode> nodesByPath,
        CancellationToken cancellationToken)
    {
        var normalizedPath = NormalizeRelativePath(relativePath);
        var csprojPath = Directory
            .EnumerateFiles(fullPath, "*.csproj", SearchOption.TopDirectoryOnly)
            .FirstOrDefault();
        var isProject = csprojPath is not null;
        var effectiveProjectPath = isProject
            ? normalizedPath
            : projectPath;

        var childDirectories = Directory
            .EnumerateDirectories(fullPath)
            .Where(path => !IgnoredDirectories.Contains(Path.GetFileName(path) ?? string.Empty))
            .OrderBy(path => Path.GetFileName(path) ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var childFiles = Directory
            .EnumerateFiles(fullPath)
            .Where(IsIncludedFile)
            .OrderBy(path => Path.GetFileName(path) ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var children = new List<RepositoryNode>();

        foreach (var directory in childDirectories)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var childRelativePath = CombineRelative(
                normalizedPath,
                Path.GetFileName(directory) ?? string.Empty);

            children.Add(await BuildDirectoryNodeAsync(
                directory,
                childRelativePath,
                normalizedPath,
                effectiveProjectPath,
                depth + 1,
                isArea: false,
                nodesByPath,
                cancellationToken));
        }

        foreach (var file in childFiles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            children.Add(await BuildFileNodeAsync(
                file,
                normalizedPath,
                effectiveProjectPath,
                depth + 1,
                nodesByPath,
                cancellationToken));
        }

        var node = new RepositoryNode
        {
            Id = BuildId(isArea ? "area" : isProject ? "project" : "folder", normalizedPath),
            Name = Path.GetFileName(fullPath) ?? string.Empty,
            RelativePath = normalizedPath,
            Kind = isArea ? "area" : isProject ? "project" : "folder",
            ArchitectureRole = ResolveArchitectureRole(normalizedPath, isProject, isArea),
            ParentPath = parentPath,
            ProjectPath = effectiveProjectPath,
            Depth = depth,
            Children = children
        };

        node.FileCount = CountFiles(node);
        nodesByPath[normalizedPath] = node;
        return node;
    }

    private async Task<RepositoryNode> BuildFileNodeAsync(
        string fullPath,
        string? parentPath,
        string? projectPath,
        int depth,
        IDictionary<string, RepositoryNode> nodesByPath,
        CancellationToken cancellationToken)
    {
        var relativePath = NormalizeRelativePath(
            Path.GetRelativePath(_dotnetRoot, fullPath));
        var extension = Path.GetExtension(fullPath);
        var lineCount = 0;
        IReadOnlyList<RepositoryCodeSymbol> symbols = [];

        if (extension.Equals(".cs", StringComparison.OrdinalIgnoreCase))
        {
            var lines = await File.ReadAllLinesAsync(fullPath, cancellationToken);
            lineCount = lines.Length;
            symbols = ExtractSymbols(relativePath, lines);
        }
        else if (IsTextFile(extension))
        {
            lineCount = await CountLinesAsync(fullPath, cancellationToken);
        }

        var node = new RepositoryNode
        {
            Id = BuildId("file", relativePath),
            Name = Path.GetFileName(fullPath) ?? string.Empty,
            RelativePath = relativePath,
            Kind = "file",
            ArchitectureRole = ResolveFileRole(relativePath),
            ParentPath = parentPath,
            ProjectPath = projectPath,
            Extension = extension,
            Depth = depth,
            FileCount = 1,
            LineCount = lineCount,
            Symbols = symbols
        };

        nodesByPath[relativePath] = node;
        return node;
    }

    private static IReadOnlyList<RepositoryCodeSymbol> ExtractSymbols(
        string relativePath,
        IReadOnlyList<string> lines)
    {
        var symbols = new List<RepositoryCodeSymbol>();
        var declaredTypeNames = new HashSet<string>(StringComparer.Ordinal);

        for (var index = 0; index < lines.Count; index++)
        {
            var line = lines[index];
            var trimmed = line.Trim();

            if (trimmed.Length == 0 ||
                trimmed.StartsWith("//", StringComparison.Ordinal) ||
                trimmed.StartsWith("[", StringComparison.Ordinal) ||
                trimmed.StartsWith("return ", StringComparison.Ordinal) ||
                trimmed.StartsWith("throw ", StringComparison.Ordinal) ||
                trimmed.StartsWith("await ", StringComparison.Ordinal) ||
                trimmed.StartsWith("new ", StringComparison.Ordinal))
            {
                continue;
            }

            var typeMatch = TypePattern().Match(line);

            if (typeMatch.Success)
            {
                var typeName = typeMatch.Groups["name"].Value;
                var rawKind = typeMatch.Groups["kind"].Value;
                declaredTypeNames.Add(typeName);
                symbols.Add(new RepositoryCodeSymbol
                {
                    Id = BuildId("symbol", $"{relativePath}:{index + 1}:{typeName}"),
                    Name = typeName,
                    Kind = NormalizeTypeKind(rawKind),
                    Signature = trimmed,
                    LineNumber = index + 1
                });
                continue;
            }

            var methodMatch = MethodPattern().Match(line);

            if (methodMatch.Success)
            {
                var methodName = methodMatch.Groups["name"].Value;

                if (IsControlKeyword(methodName))
                {
                    continue;
                }

                var kind = declaredTypeNames.Contains(methodName)
                    ? "constructor"
                    : "method";

                symbols.Add(new RepositoryCodeSymbol
                {
                    Id = BuildId("symbol", $"{relativePath}:{index + 1}:{methodName}"),
                    Name = methodName,
                    Kind = kind,
                    Signature = trimmed,
                    LineNumber = index + 1
                });
                continue;
            }

            var propertyMatch = PropertyPattern().Match(line);

            if (propertyMatch.Success)
            {
                var propertyName = propertyMatch.Groups["name"].Value;
                symbols.Add(new RepositoryCodeSymbol
                {
                    Id = BuildId("symbol", $"{relativePath}:{index + 1}:{propertyName}"),
                    Name = propertyName,
                    Kind = "property",
                    Signature = trimmed,
                    LineNumber = index + 1
                });
            }
        }

        return symbols
            .OrderBy(symbol => symbol.LineNumber)
            .ToArray();
    }

    private static RepositoryMetrics BuildMetrics(IReadOnlyList<RepositoryNode> areas)
    {
        var allNodes = areas
            .SelectMany(Flatten)
            .ToArray();

        return new RepositoryMetrics
        {
            Areas = areas.Count,
            Projects = allNodes.Count(node => node.Kind.Equals("project", StringComparison.OrdinalIgnoreCase)),
            Folders = allNodes.Count(node => node.Kind.Equals("folder", StringComparison.OrdinalIgnoreCase)),
            Files = allNodes.Count(node => node.IsFile),
            CSharpFiles = allNodes.Count(node =>
                node.IsFile && node.Extension.Equals(".cs", StringComparison.OrdinalIgnoreCase)),
            Symbols = allNodes.Sum(node => node.Symbols.Count)
        };
    }

    private static IEnumerable<RepositoryNode> Flatten(RepositoryNode node)
    {
        yield return node;

        foreach (var child in node.Children)
        {
            foreach (var descendant in Flatten(child))
            {
                yield return descendant;
            }
        }
    }

    private static int CountFiles(RepositoryNode node) =>
        node.IsFile
            ? 1
            : node.Children.Sum(CountFiles);

    private static bool IsIncludedFile(string path)
    {
        var fileName = Path.GetFileName(path) ?? string.Empty;

        if (fileName.Equals(".gitignore", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return IncludedExtensions.Contains(Path.GetExtension(path) ?? string.Empty);
    }

    private static bool IsTextFile(string extension) =>
        IncludedExtensions.Contains(extension);

    private static async Task<int> CountLinesAsync(
        string filePath,
        CancellationToken cancellationToken)
    {
        var count = 0;

        using var stream = File.OpenRead(filePath);
        using var reader = new StreamReader(stream);

        while (await reader.ReadLineAsync(cancellationToken) is not null)
        {
            count++;
        }

        return count;
    }

    private static string ResolveArchitectureRole(
        string path,
        bool isProject,
        bool isArea)
    {
        if (isArea)
        {
            return path switch
            {
                "src" => "Productive implementation",
                "tests" => "Verification",
                "docs" => "Architecture documentation",
                "tools" => "Educational tooling",
                _ => "Repository"
            };
        }

        if (!isProject)
        {
            return "Physical folder";
        }

        if (path.StartsWith("tests/", StringComparison.OrdinalIgnoreCase)) return "Tests";
        if (path.StartsWith("tools/", StringComparison.OrdinalIgnoreCase)) return "Tooling";
        if (path.Contains(".Domain", StringComparison.OrdinalIgnoreCase)) return "Domain";
        if (path.Contains(".Application", StringComparison.OrdinalIgnoreCase)) return "Application / Ports";
        if (path.Contains("Adapters.Inbound", StringComparison.OrdinalIgnoreCase)) return "Inbound Adapter";
        if (path.Contains("Adapters.Outbound", StringComparison.OrdinalIgnoreCase)) return "Outbound Adapter";
        if (path.Contains("Host.", StringComparison.OrdinalIgnoreCase)) return "Composition Root";

        return "Project";
    }

    private static string ResolveFileRole(string path)
    {
        if (path.StartsWith("tests/", StringComparison.OrdinalIgnoreCase)) return "Test";
        if (path.StartsWith("docs/", StringComparison.OrdinalIgnoreCase)) return "Documentation";
        if (path.StartsWith("tools/", StringComparison.OrdinalIgnoreCase)) return "Tooling";
        if (path.Contains("/Ports/Inbound/", StringComparison.OrdinalIgnoreCase)) return "Input Port";
        if (path.Contains("/Ports/Outbound/", StringComparison.OrdinalIgnoreCase)) return "Output Port";
        if (path.Contains("Adapters.Inbound", StringComparison.OrdinalIgnoreCase)) return "Inbound Adapter";
        if (path.Contains("Adapters.Outbound", StringComparison.OrdinalIgnoreCase)) return "Outbound Adapter";
        if (path.Contains(".Domain/", StringComparison.OrdinalIgnoreCase)) return "Domain";
        if (path.Contains(".Application/", StringComparison.OrdinalIgnoreCase)) return "Application";
        if (path.Contains("Host.", StringComparison.OrdinalIgnoreCase)) return "Composition Root";

        return "Repository file";
    }

    private static int GetSearchRank(RepositoryNode node, string query)
    {
        if (node.Name.Equals(query, StringComparison.OrdinalIgnoreCase)) return 0;
        if (node.Name.StartsWith(query, StringComparison.OrdinalIgnoreCase)) return 1;
        if (node.Symbols.Any(symbol => symbol.Name.Equals(query, StringComparison.OrdinalIgnoreCase))) return 2;
        if (node.Name.Contains(query, StringComparison.OrdinalIgnoreCase)) return 3;
        return 4;
    }

    private static bool IsControlKeyword(string value) =>
        value is "if" or "for" or "foreach" or "while" or "switch" or "catch" or "using" or "lock";

    private static string NormalizeTypeKind(string rawKind)
    {
        if (rawKind.StartsWith("record", StringComparison.Ordinal)) return "record";
        return rawKind;
    }

    private static string CombineRelative(string left, string right) =>
        NormalizeRelativePath($"{left}/{right}");

    private static string NormalizeRelativePath(string path) =>
        path.Replace('\\', '/').Trim('/');

    private static string BuildId(string prefix, string value)
    {
        var safe = NonAlphaNumericPattern()
            .Replace(value.ToLowerInvariant(), "-")
            .Trim('-');

        return $"{prefix}-{safe}";
    }
    [GeneratedRegex(
        @"^\s*(?:(?:public|internal|private|protected)\s+)?(?:(?:sealed|static|abstract|partial|readonly)\s+)*(?<kind>class|interface|record(?:\s+(?:class|struct))?|struct|enum)\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)",
        RegexOptions.CultureInvariant)]
    private static partial Regex TypePattern();

    [GeneratedRegex(
        @"^\s*(?:(?:public|internal|private|protected)\s+)?(?:(?:static|async|virtual|override|sealed|abstract|partial|new)\s+)*(?:[A-Za-z_][A-Za-z0-9_<>,\[\]?\.]*\s+)+(?<name>[A-Za-z_][A-Za-z0-9_]*)\s*\(",
        RegexOptions.CultureInvariant)]
    private static partial Regex MethodPattern();


    [GeneratedRegex(
        @"^\s*(?:(?:public|internal|private|protected)\s+)?(?:(?:static|virtual|override|abstract|new|required)\s+)*(?:[A-Za-z_][A-Za-z0-9_<>,\[\]?\.]*\s+)+(?<name>[A-Za-z_][A-Za-z0-9_]*)\s*(?:=>|\{)",
        RegexOptions.CultureInvariant)]
    private static partial Regex PropertyPattern();

    [GeneratedRegex(@"[^a-z0-9]+", RegexOptions.CultureInvariant)]
    private static partial Regex NonAlphaNumericPattern();

}
