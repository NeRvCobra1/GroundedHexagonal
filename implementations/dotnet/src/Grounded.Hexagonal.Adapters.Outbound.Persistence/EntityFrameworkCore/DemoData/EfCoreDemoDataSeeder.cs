using Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore.Mapping;
using Grounded.Hexagonal.Domain.Crafting;
using Grounded.Hexagonal.Domain.Inventory;
using Grounded.Hexagonal.Domain.Items;
using Microsoft.EntityFrameworkCore;
using InventoryAggregate = Grounded.Hexagonal.Domain.Inventory.Inventory;

namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore.DemoData;

/// <summary>
/// Seeds stable local demo data without overwriting existing state.
/// </summary>
/// <remarks>
/// This helper belongs to the concrete persistence adapter because it writes
/// directly to the SQLite persistence model. It is intentionally not an
/// Application port and it must never reset an existing demo inventory.
/// </remarks>
public sealed class EfCoreDemoDataSeeder
{
    private readonly GroundedDbContextFactory _dbContextFactory;

    public EfCoreDemoDataSeeder(
        SqlitePersistenceOptions persistenceOptions)
    {
        _dbContextFactory =
            new GroundedDbContextFactory(persistenceOptions);
    }

    public async Task SeedIfMissingAsync(
        CancellationToken cancellationToken = default)
    {
        await using var dbContext =
            _dbContextFactory.CreateDbContext();

        var inventoryExists =
            await dbContext.Inventories.AnyAsync(
                record => record.PlayerId == EfCoreDemoDataIds.PlayerId,
                cancellationToken);

        if (!inventoryExists)
        {
            dbContext.Inventories.Add(
                DomainPersistenceMapper.ToRecord(
                    CreateDemoInventory()));
        }

        var recipeExists =
            await dbContext.Recipes.AnyAsync(
                record => record.RecipeId == EfCoreDemoDataIds.MintMaceRecipeId,
                cancellationToken);

        if (!recipeExists)
        {
            dbContext.Recipes.Add(
                DomainPersistenceMapper.ToRecord(
                    CreateMintMaceRecipe()));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static InventoryAggregate CreateDemoInventory()
    {
        var inventory =
            new InventoryAggregate(
                PlayerId.From(EfCoreDemoDataIds.PlayerId));

        inventory.Add(
            ItemId.From(EfCoreDemoDataIds.MintShardItemId),
            12);

        inventory.Add(
            ItemId.From(EfCoreDemoDataIds.ToughGunkItemId),
            5);

        inventory.Add(
            ItemId.From(EfCoreDemoDataIds.FlowerPetalItemId),
            8);

        return inventory;
    }

    private static Recipe CreateMintMaceRecipe()
    {
        return new Recipe(
            RecipeId.From(EfCoreDemoDataIds.MintMaceRecipeId),
            ItemId.From(EfCoreDemoDataIds.MintMaceItemId),
            1,
            [
                new IngredientRequirement(
                    ItemId.From(EfCoreDemoDataIds.MintShardItemId),
                    10),
                new IngredientRequirement(
                    ItemId.From(EfCoreDemoDataIds.ToughGunkItemId),
                    5),
                new IngredientRequirement(
                    ItemId.From(EfCoreDemoDataIds.FlowerPetalItemId),
                    3)
            ]);
    }
}
