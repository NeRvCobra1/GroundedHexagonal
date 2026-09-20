using System.Reflection;
using Grounded.Hexagonal.Adapters.Inbound.Http.CraftItem;
using Grounded.Hexagonal.Adapters.Inbound.Worker.FoodSpoilage;
using Grounded.Hexagonal.Adapters.Outbound.Time;
using Grounded.Hexagonal.Adapters.Outbound.Persistence.InMemory;
using Grounded.Hexagonal.Application.UseCases.CraftItem;
using Grounded.Hexagonal.Domain.Items;

namespace Grounded.Hexagonal.ArchitectureTests;

/// <summary>
/// Protects technology and assembly boundaries by inspecting the references
/// emitted into the compiled assemblies.
/// </summary>
public sealed class AssemblyDependencyTests
{
    private static readonly string[] CoreForbiddenPrefixes =
    [
        "Microsoft.AspNetCore",
        "Microsoft.EntityFrameworkCore",
        "Grounded.Hexagonal.Adapters",
        "Grounded.Hexagonal.Host"
    ];

    [Fact]
    public void Domain_Should_Not_Reference_Frameworks_Or_Outer_Assemblies()
    {
        var assembly = typeof(ItemId).Assembly;

        AssertDoesNotReferenceAny(
            assembly,
            CoreForbiddenPrefixes);
    }

    [Fact]
    public void Application_Should_Not_Reference_Frameworks_Or_Outer_Assemblies()
    {
        var assembly = typeof(CraftItemHandler).Assembly;

        AssertDoesNotReferenceAny(
            assembly,
            CoreForbiddenPrefixes);
    }

    [Fact]
    public void Http_Adapter_Should_Reference_AspNetCore()
    {
        var assembly = typeof(CraftItemHttpRequest).Assembly;

        var references = GetReferenceNames(assembly);

        Assert.Contains(
            references,
            reference => reference.StartsWith(
                "Microsoft.AspNetCore",
                StringComparison.Ordinal));
    }

    [Fact]
    public void Persistence_Adapter_Should_Not_Reference_AspNetCore()
    {
        var assembly = typeof(InMemoryInventoryRepository).Assembly;

        AssertDoesNotReferenceAny(
            assembly,
            ["Microsoft.AspNetCore"]);
    }


    [Fact]
    public void Worker_Adapter_Should_Not_Reference_AspNetCore()
    {
        var assembly = typeof(FoodSpoilageWorker).Assembly;

        AssertDoesNotReferenceAny(
            assembly,
            ["Microsoft.AspNetCore"]);
    }

    [Fact]
    public void Time_Adapter_Should_Not_Reference_AspNetCore_Or_EntityFrameworkCore()
    {
        var assembly = typeof(SystemClock).Assembly;

        AssertDoesNotReferenceAny(
            assembly,
            [
                "Microsoft.AspNetCore",
                "Microsoft.EntityFrameworkCore"
            ]);
    }

    private static void AssertDoesNotReferenceAny(
        Assembly assembly,
        IEnumerable<string> forbiddenPrefixes)
    {
        var references = GetReferenceNames(assembly);

        foreach (var prefix in forbiddenPrefixes)
        {
            Assert.DoesNotContain(
                references,
                reference => reference.StartsWith(
                    prefix,
                    StringComparison.Ordinal));
        }
    }

    private static string[] GetReferenceNames(Assembly assembly)
    {
        return assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Cast<string>()
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
    }
}
