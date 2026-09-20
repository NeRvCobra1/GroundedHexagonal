using Grounded.Hexagonal.Domain.Food;

namespace Grounded.Hexagonal.Domain.Tests.Food;

public sealed class FoodSpoilageTests
{
    [Fact]
    public void FoodId_Should_Reject_Empty_Guid()
    {
        Assert.Throws<ArgumentException>(
            () => FoodId.From(Guid.Empty));
    }

    [Fact]
    public void AdvanceSpoilage_Should_Keep_Food_Fresh_Before_Spoilage_Time()
    {
        var spoilsAt = new DateTimeOffset(
            2026, 9, 20, 12, 0, 0, TimeSpan.Zero);

        var food = new Domain.Food.Food(
            FoodId.New(),
            spoilsAt);

        var changed = food.AdvanceSpoilage(
            spoilsAt.AddSeconds(-1));

        Assert.False(changed);
        Assert.False(food.IsSpoiled);
        Assert.Equal(FoodSpoilageState.Fresh, food.State);
    }

    [Fact]
    public void AdvanceSpoilage_Should_Spoil_Food_When_Time_Reaches_Spoilage_Time()
    {
        var spoilsAt = new DateTimeOffset(
            2026, 9, 20, 12, 0, 0, TimeSpan.Zero);

        var food = new Domain.Food.Food(
            FoodId.New(),
            spoilsAt);

        var changed = food.AdvanceSpoilage(spoilsAt);

        Assert.True(changed);
        Assert.True(food.IsSpoiled);
        Assert.Equal(FoodSpoilageState.Spoiled, food.State);
    }

    [Fact]
    public void AdvanceSpoilage_Should_Not_Report_A_Second_Change_When_Already_Spoiled()
    {
        var spoilsAt = new DateTimeOffset(
            2026, 9, 20, 12, 0, 0, TimeSpan.Zero);

        var food = new Domain.Food.Food(
            FoodId.New(),
            spoilsAt);

        Assert.True(food.AdvanceSpoilage(spoilsAt));
        Assert.False(food.AdvanceSpoilage(spoilsAt.AddHours(1)));

        Assert.True(food.IsSpoiled);
    }
}
