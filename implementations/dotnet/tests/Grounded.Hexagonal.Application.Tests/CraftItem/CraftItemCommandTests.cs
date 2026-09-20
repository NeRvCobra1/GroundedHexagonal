using Grounded.Hexagonal.Application.UseCases.CraftItem;
using Grounded.Hexagonal.Domain.Crafting;
using Grounded.Hexagonal.Domain.Inventory;

namespace Grounded.Hexagonal.Application.Tests.CraftItem;

public sealed class CraftItemCommandTests
{
    [Fact]
    public void Constructor_Should_Reject_Null_PlayerId()
    {
        Assert.Throws<ArgumentNullException>(
            () => new CraftItemCommand(
                null!,
                RecipeId.New()));
    }

    [Fact]
    public void Constructor_Should_Reject_Null_RecipeId()
    {
        Assert.Throws<ArgumentNullException>(
            () => new CraftItemCommand(
                PlayerId.New(),
                null!));
    }
}
