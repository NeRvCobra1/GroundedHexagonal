namespace Grounded.Hexagonal.Adapters.Inbound.Http.CraftItem;

/// <summary>
/// HTTP transport model for a CraftItem request.
/// </summary>
/// <remarks>
/// This type belongs to the HTTP adapter.
///
/// It is intentionally different from CraftItemCommand so that transport
/// concerns do not leak into Application.
/// </remarks>
public sealed record CraftItemHttpRequest(
    Guid PlayerId,
    Guid RecipeId);
