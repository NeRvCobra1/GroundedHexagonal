namespace Grounded.Hexagonal.Adapters.Inbound.Http.CraftItem;

/// <summary>
/// HTTP transport model returned by the CraftItem endpoint.
/// </summary>
public sealed record CraftItemHttpResponse(
    string Status);
