using Grounded.Hexagonal.Visualizer.Models;

namespace Grounded.Hexagonal.Visualizer.Services;

public sealed class SourceSnippetService
{
    private readonly string _dotnetRoot;

    public SourceSnippetService(IWebHostEnvironment environment)
    {
        _dotnetRoot = Path.GetFullPath(
            Path.Combine(
                environment.ContentRootPath,
                "..",
                ".."));
    }

    public async Task<SourceSnippet> ReadAsync(
        CodeReference? codeReference,
        CancellationToken cancellationToken = default)
    {
        if (codeReference is null ||
            string.IsNullOrWhiteSpace(codeReference.FilePath))
        {
            return SourceSnippet.Empty(
                "Este paso es conceptual o externo y no apunta a un archivo C# específico.");
        }

        var fullPath = ResolveSafePath(codeReference.FilePath);

        if (fullPath is null)
        {
            return SourceSnippet.Empty(
                "La referencia de código está fuera de implementations/dotnet.");
        }

        if (!File.Exists(fullPath))
        {
            return SourceSnippet.Empty(
                $"No se encontró el archivo '{codeReference.FilePath}'.");
        }

        var lines = await File.ReadAllLinesAsync(
            fullPath,
            cancellationToken);

        var startIndex = FindStartIndex(
            lines,
            codeReference.StartContains);

        if (startIndex < 0)
        {
            return SourceSnippet.Empty(
                $"Se encontró el archivo, pero no el ancla '{codeReference.StartContains}'.");
        }

        var takeLines = Math.Clamp(codeReference.TakeLines, 1, 120);
        var selectedLines = lines
            .Skip(startIndex)
            .Take(takeLines)
            .Select((text, index) =>
                new SourceSnippetLine(
                    startIndex + index + 1,
                    text))
            .ToArray();

        return new SourceSnippet
        {
            IsAvailable = true,
            FilePath = codeReference.FilePath,
            Lines = selectedLines
        };
    }


    public async Task<SourceSnippet> ReadFileSectionAsync(
        string filePath,
        int startLine = 1,
        int takeLines = 80,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return SourceSnippet.Empty("No se seleccionó un archivo.");
        }

        var resolvedPath = ResolveSafePath(filePath);

        if (resolvedPath is null)
        {
            return SourceSnippet.Empty("La referencia de código está fuera de implementations/dotnet.");
        }

        if (!File.Exists(resolvedPath))
        {
            return SourceSnippet.Empty($"No se encontró el archivo '{filePath}'.");
        }

        var lines = await File.ReadAllLinesAsync(resolvedPath, cancellationToken);
        var safeStartLine = Math.Clamp(startLine, 1, Math.Max(1, lines.Length));
        var safeTake = Math.Clamp(takeLines, 1, 160);
        var startIndex = safeStartLine - 1;
        var selectedLines = lines
            .Skip(startIndex)
            .Take(safeTake)
            .Select((text, index) =>
                new SourceSnippetLine(
                    startIndex + index + 1,
                    text))
            .ToArray();

        return new SourceSnippet
        {
            IsAvailable = true,
            FilePath = filePath.Replace('\\', '/'),
            Lines = selectedLines
        };
    }

    public Task<SourceSnippet> ReadAroundLineAsync(
        string filePath,
        int lineNumber,
        int contextLines = 14,
        CancellationToken cancellationToken = default)
    {
        var safeContext = Math.Clamp(contextLines, 3, 50);
        var startLine = Math.Max(1, lineNumber - safeContext);

        return ReadFileSectionAsync(
            filePath,
            startLine,
            safeContext * 2 + 1,
            cancellationToken);
    }

    private string? ResolveSafePath(string relativePath)
    {
        var fullPath = Path.GetFullPath(
            Path.Combine(
                _dotnetRoot,
                relativePath.Replace('/', Path.DirectorySeparatorChar)));

        var rootWithSeparator = _dotnetRoot.EndsWith(Path.DirectorySeparatorChar)
            ? _dotnetRoot
            : _dotnetRoot + Path.DirectorySeparatorChar;

        if (!fullPath.Equals(_dotnetRoot, StringComparison.OrdinalIgnoreCase) &&
            !fullPath.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return fullPath;
    }

    private static int FindStartIndex(
        IReadOnlyList<string> lines,
        string startContains)
    {
        if (string.IsNullOrWhiteSpace(startContains))
        {
            return 0;
        }

        for (var index = 0; index < lines.Count; index++)
        {
            if (lines[index].Contains(
                    startContains,
                    StringComparison.Ordinal))
            {
                return index;
            }
        }

        return -1;
    }
}
