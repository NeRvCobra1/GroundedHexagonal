using Grounded.Hexagonal.Domain.Crafting;
using Grounded.Hexagonal.Domain.Inventory;
using Grounded.Hexagonal.Domain.Items;

namespace Grounded.Hexagonal.Domain.Tests.Identifiers;

public sealed class IdentifierTests
{
    [Fact]
    public void ItemId_Should_Reject_Empty_Guid()
    {
        Assert.Throws<ArgumentException>(() => ItemId.From(Guid.Empty));
    }

    [Fact]
    public void PlayerId_Should_Reject_Empty_Guid()
    {
        Assert.Throws<ArgumentException>(() => PlayerId.From(Guid.Empty));
    }

    [Fact]
    public void RecipeId_Should_Reject_Empty_Guid()
    {
        Assert.Throws<ArgumentException>(() => RecipeId.From(Guid.Empty));
    }
}
