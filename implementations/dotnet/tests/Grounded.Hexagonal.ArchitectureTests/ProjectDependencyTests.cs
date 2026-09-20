using System.Xml.Linq;

namespace Grounded.Hexagonal.ArchitectureTests;

public sealed class ProjectDependencyTests
{
    private const string Domain = "Grounded.Hexagonal.Domain";
    private const string Application = "Grounded.Hexagonal.Application";
    private const string InboundHttp = "Grounded.Hexagonal.Adapters.Inbound.Http";
    private const string InboundWorker = "Grounded.Hexagonal.Adapters.Inbound.Worker";
    private const string OutboundPersistence = "Grounded.Hexagonal.Adapters.Outbound.Persistence";
    private const string HostApi = "Grounded.Hexagonal.Host.Api";
    private const string HostWorker = "Grounded.Hexagonal.Host.Worker";

    [Fact]
    public void Domain_Should_Not_Reference_Other_Productive_Projects()
    {
        AssertProjectReferences(
            Domain);
    }

    [Fact]
    public void Application_Should_Reference_Only_Domain()
    {
        AssertProjectReferences(
            Application,
            Domain);
    }

    [Fact]
    public void InboundHttp_Should_Reference_Only_Application()
    {
        AssertProjectReferences(
            InboundHttp,
            Application);
    }

    [Fact]
    public void InboundWorker_Should_Reference_Only_Application()
    {
        AssertProjectReferences(
            InboundWorker,
            Application);
    }

    [Fact]
    public void OutboundPersistence_Should_Reference_Only_Application_And_Domain()
    {
        AssertProjectReferences(
            OutboundPersistence,
            Application,
            Domain);
    }

    [Fact]
    public void HostApi_Should_Reference_Only_Its_Composition_Dependencies()
    {
        AssertProjectReferences(
            HostApi,
            Application,
            InboundHttp,
            OutboundPersistence);
    }

    [Fact]
    public void HostWorker_Should_Reference_Only_Its_Composition_Dependencies()
    {
        AssertProjectReferences(
            HostWorker,
            Application,
            InboundWorker,
            OutboundPersistence);
    }

    private static void AssertProjectReferences(
        string projectName,
        params string[] expectedReferences)
    {
        var actualReferences = GetProjectReferences(projectName)
            .OrderBy(reference => reference, StringComparer.Ordinal)
            .ToArray();

        var expected = expectedReferences
            .OrderBy(reference => reference, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expected, actualReferences);
    }

    private static IReadOnlyCollection<string> GetProjectReferences(string projectName)
    {
        var projectPath = GetProjectPath(projectName);
        var projectDocument = XDocument.Load(projectPath);

        return projectDocument
            .Descendants()
            .Where(element => element.Name.LocalName == "ProjectReference")
            .Select(element => element.Attribute("Include")?.Value)
            .Where(include => !string.IsNullOrWhiteSpace(include))
            .Select(include => Path.GetFileNameWithoutExtension(include!))
            .ToArray();
    }

    private static string GetProjectPath(string projectName)
    {
        var implementationRoot = FindImplementationRoot();

        var projectPath = Path.Combine(
            implementationRoot,
            "src",
            projectName,
            $"{projectName}.csproj");

        Assert.True(
            File.Exists(projectPath),
            $"Could not find project file: {projectPath}");

        return projectPath;
    }

    private static string FindImplementationRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var solutionPath = Path.Combine(
                directory.FullName,
                "Grounded.Hexagonal.slnx");

            var buildPropsPath = Path.Combine(
                directory.FullName,
                "Directory.Build.props");

            if (File.Exists(solutionPath) && File.Exists(buildPropsPath))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the implementations/dotnet root. " +
            "Expected to find Grounded.Hexagonal.slnx and Directory.Build.props.");
    }
}
