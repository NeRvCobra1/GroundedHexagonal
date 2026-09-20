using System.Xml.Linq;

namespace Grounded.Hexagonal.ArchitectureTests;

/// <summary>
/// Protects technology boundaries expressed in the project files.
/// </summary>
public sealed class ProjectTechnologyBoundaryTests
{
    private const string Domain = "Grounded.Hexagonal.Domain";
    private const string Application = "Grounded.Hexagonal.Application";
    private const string InboundHttp = "Grounded.Hexagonal.Adapters.Inbound.Http";

    [Fact]
    public void Domain_Should_Not_Declare_Package_Or_Framework_References()
    {
        var document = LoadProject(Domain);

        Assert.Empty(GetIncludes(document, "PackageReference"));
        Assert.Empty(GetIncludes(document, "FrameworkReference"));
    }

    [Fact]
    public void Application_Should_Not_Declare_Package_Or_Framework_References()
    {
        var document = LoadProject(Application);

        Assert.Empty(GetIncludes(document, "PackageReference"));
        Assert.Empty(GetIncludes(document, "FrameworkReference"));
    }

    [Fact]
    public void Http_Adapter_Should_Declare_AspNetCore_FrameworkReference()
    {
        var document = LoadProject(InboundHttp);

        var frameworkReferences =
            GetIncludes(document, "FrameworkReference");

        Assert.Equal(
            ["Microsoft.AspNetCore.App"],
            frameworkReferences);
    }

    private static XDocument LoadProject(string projectName)
    {
        var root = FindImplementationRoot();

        var path = Path.Combine(
            root,
            "src",
            projectName,
            $"{projectName}.csproj");

        Assert.True(
            File.Exists(path),
            $"Could not find project file: {path}");

        return XDocument.Load(path);
    }

    private static string[] GetIncludes(
        XDocument document,
        string elementName)
    {
        return document
            .Descendants()
            .Where(element => element.Name.LocalName == elementName)
            .Select(element => element.Attribute("Include")?.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Cast<string>()
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();
    }

    private static string FindImplementationRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var solution = Path.Combine(
                directory.FullName,
                "Grounded.Hexagonal.slnx");

            var buildProps = Path.Combine(
                directory.FullName,
                "Directory.Build.props");

            if (File.Exists(solution) && File.Exists(buildProps))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the implementations/dotnet root.");
    }
}
