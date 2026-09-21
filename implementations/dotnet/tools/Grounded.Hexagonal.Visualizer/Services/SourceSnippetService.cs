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

        var fullPath = Path.GetFullPath(
            Path.Combine(
                _dotnetRoot,
                codeReference.FilePath.Replace('/', Path.DirectorySeparatorChar)));

        if (!fullPath.StartsWith(
                _dotnetRoot,
                StringComparison.OrdinalIgnoreCase))
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
