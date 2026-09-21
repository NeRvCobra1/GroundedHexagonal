namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore.DemoData;

/// <summary>
/// Stable identifiers used only by the local educational demo data.
/// </summary>
public static class EfCoreDemoDataIds
{
    public static readonly Guid PlayerId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static readonly Guid MintMaceRecipeId =
        Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static readonly Guid MintShardItemId =
        Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static readonly Guid ToughGunkItemId =
        Guid.Parse("44444444-4444-4444-4444-444444444444");

    public static readonly Guid FlowerPetalItemId =
        Guid.Parse("55555555-5555-5555-5555-555555555555");

    public static readonly Guid MintMaceItemId =
        Guid.Parse("66666666-6666-6666-6666-666666666666");
}
