using Grounded.Hexagonal.Application.UseCases.CraftItem;

namespace Grounded.Hexagonal.Application.Ports.Inbound;

/// <summary>
/// Inbound port for the CraftItem use case.
/// </summary>
/// <remarks>
/// Architecture ID: PORT-IN-CRAFT-001
/// Use Case: UC-CRAFT-001
///
/// This C# interface is the .NET representation of the architectural
/// inbound port. Hexagonal Architecture does not require ports to be
/// represented as interfaces in every language or implementation.
/// </remarks>
public interface ICraftItemUseCase
{
    Task<CraftItemResult> ExecuteAsync(
        CraftItemCommand command,
        CancellationToken cancellationToken = default);
}
