using Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore.Models;
using Grounded.Hexagonal.Domain.Crafting;
using Grounded.Hexagonal.Domain.Food;
using Grounded.Hexagonal.Domain.Inventory;
using Grounded.Hexagonal.Domain.Items;
using InventoryAggregate = Grounded.Hexagonal.Domain.Inventory.Inventory;

namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore.Mapping;

internal static class DomainPersistenceMapper
{
    public static InventoryAggregate ToDomain(
        InventoryRecord record)
    {
        var inventory = new InventoryAggregate(
            PlayerId.From(record.PlayerId));

        foreach (var item in record.Items)
        {
            inventory.Add(
                ItemId.From(item.ItemId),
                item.Quantity);
        }

        return inventory;
    }

    public static Recipe ToDomain(
        RecipeRecord record)
    {
        var ingredients = record.Ingredients
            .OrderBy(ingredient => ingredient.Position)
            .Select(ingredient => new IngredientRequirement(
                ItemId.From(ingredient.ItemId),
                ingredient.Quantity))
            .ToArray();

        return new Recipe(
            RecipeId.From(record.RecipeId),
            ItemId.From(record.ResultItemId),
            record.ResultQuantity,
            ingredients);
    }

    public static Food ToDomain(
        FoodRecord record)
    {
        var utcDateTime = DateTime.SpecifyKind(
            record.SpoilsAtUtc,
            DateTimeKind.Utc);

        return Food.Restore(
            FoodId.From(record.FoodId),
            new DateTimeOffset(utcDateTime),
            (FoodSpoilageState)record.State);
    }

    public static RecipeRecord ToRecord(
        Recipe recipe)
    {
        return new RecipeRecord
        {
            RecipeId = recipe.Id.Value,
            ResultItemId = recipe.ResultItemId.Value,
            ResultQuantity = recipe.ResultQuantity,
            Ingredients = recipe.Ingredients
                .Select((ingredient, position) =>
                    new RecipeIngredientRecord
                    {
                        RecipeId = recipe.Id.Value,
                        Position = position,
                        ItemId = ingredient.ItemId.Value,
                        Quantity = ingredient.Quantity
                    })
                .ToList()
        };
    }

    public static FoodRecord ToRecord(
        Food food)
    {
        return new FoodRecord
        {
            FoodId = food.Id.Value,
            SpoilsAtUtc = food.SpoilsAt.UtcDateTime,
            State = (int)food.State
        };
    }
}
