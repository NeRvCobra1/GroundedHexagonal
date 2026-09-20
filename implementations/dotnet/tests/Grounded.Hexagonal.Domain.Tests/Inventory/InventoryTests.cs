using Grounded.Hexagonal.Domain.Crafting;
using Grounded.Hexagonal.Domain.Inventory;
using Grounded.Hexagonal.Domain.Items;
using InventoryAggregate = Grounded.Hexagonal.Domain.Inventory.Inventory;

namespace Grounded.Hexagonal.Domain.Tests.Inventory;

public sealed class InventoryTests
{
    [Fact]
    public void Add_Should_Increase_Item_Quantity()
    {
        var inventory = new InventoryAggregate(PlayerId.New());
        var itemId = ItemId.New();

        inventory.Add(itemId, 4);
        inventory.Add(itemId, 3);

        Assert.Equal(7, inventory.GetQuantity(itemId));
    }

    [Fact]
    public void GetItems_Should_Return_Read_Only_Snapshot_Of_Current_Contents()
    {
        var inventory = new InventoryAggregate(PlayerId.New());
        var firstItem = ItemId.New();
        var secondItem = ItemId.New();

        inventory.Add(firstItem, 4);
        inventory.Add(secondItem, 7);

        var snapshot = inventory.GetItems();

        Assert.Equal(2, snapshot.Count);

        Assert.Contains(
            snapshot,
            item => item.ItemId == firstItem && item.Quantity == 4);

        Assert.Contains(
            snapshot,
            item => item.ItemId == secondItem && item.Quantity == 7);

        inventory.Add(firstItem, 3);

        var firstItemInSnapshot = Assert.Single(
            snapshot,
            item => item.ItemId == firstItem);

        Assert.Equal(4, firstItemInSnapshot.Quantity);
        Assert.Equal(7, inventory.GetQuantity(firstItem));
    }

    [Fact]
    public void Craft_Should_Consume_All_Ingredients_And_Add_Result()
    {
        var mintShard = ItemId.New();
        var toughGunk = ItemId.New();
        var flowerPetal = ItemId.New();
        var mintMace = ItemId.New();

        var recipe = new Recipe(
            RecipeId.New(),
            mintMace,
            1,
            [
                new IngredientRequirement(mintShard, 10),
                new IngredientRequirement(toughGunk, 5),
                new IngredientRequirement(flowerPetal, 3)
            ]);

        var inventory = new InventoryAggregate(PlayerId.New());

        inventory.Add(mintShard, 12);
        inventory.Add(toughGunk, 5);
        inventory.Add(flowerPetal, 8);

        inventory.Craft(recipe);

        Assert.Equal(2, inventory.GetQuantity(mintShard));
        Assert.Equal(0, inventory.GetQuantity(toughGunk));
        Assert.Equal(5, inventory.GetQuantity(flowerPetal));
        Assert.Equal(1, inventory.GetQuantity(mintMace));
    }

    [Fact]
    public void Craft_Should_Not_Modify_Inventory_When_Any_Ingredient_Is_Insufficient()
    {
        var mintShard = ItemId.New();
        var toughGunk = ItemId.New();
        var flowerPetal = ItemId.New();
        var mintMace = ItemId.New();

        var recipe = new Recipe(
            RecipeId.New(),
            mintMace,
            1,
            [
                new IngredientRequirement(mintShard, 10),
                new IngredientRequirement(toughGunk, 5),
                new IngredientRequirement(flowerPetal, 3)
            ]);

        var inventory = new InventoryAggregate(PlayerId.New());

        inventory.Add(mintShard, 10);
        inventory.Add(toughGunk, 4);
        inventory.Add(flowerPetal, 3);

        Assert.Throws<InsufficientIngredientsException>(
            () => inventory.Craft(recipe));

        Assert.Equal(10, inventory.GetQuantity(mintShard));
        Assert.Equal(4, inventory.GetQuantity(toughGunk));
        Assert.Equal(3, inventory.GetQuantity(flowerPetal));
        Assert.Equal(0, inventory.GetQuantity(mintMace));
    }

    [Fact]
    public void Craft_Should_Combine_Repeated_Requirements_Before_Validation()
    {
        var material = ItemId.New();
        var result = ItemId.New();

        var recipe = new Recipe(
            RecipeId.New(),
            result,
            1,
            [
                new IngredientRequirement(material, 2),
                new IngredientRequirement(material, 3)
            ]);

        var inventory = new InventoryAggregate(PlayerId.New());
        inventory.Add(material, 4);

        Assert.Throws<InsufficientIngredientsException>(
            () => inventory.Craft(recipe));

        Assert.Equal(4, inventory.GetQuantity(material));
        Assert.Equal(0, inventory.GetQuantity(result));
    }
}
