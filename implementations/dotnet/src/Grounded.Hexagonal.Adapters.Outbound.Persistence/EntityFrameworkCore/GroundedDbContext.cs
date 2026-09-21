using Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;

namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore;

public sealed class GroundedDbContext : DbContext
{
    public GroundedDbContext(
        DbContextOptions<GroundedDbContext> options)
        : base(options)
    {
    }

    internal DbSet<InventoryRecord> Inventories => Set<InventoryRecord>();

    internal DbSet<InventoryItemRecord> InventoryItems => Set<InventoryItemRecord>();

    internal DbSet<RecipeRecord> Recipes => Set<RecipeRecord>();

    internal DbSet<RecipeIngredientRecord> RecipeIngredients => Set<RecipeIngredientRecord>();

    internal DbSet<FoodRecord> Foods => Set<FoodRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InventoryRecord>(entity =>
        {
            entity.ToTable("Inventories");
            entity.HasKey(record => record.PlayerId);

            entity.HasMany(record => record.Items)
                .WithOne(item => item.Inventory)
                .HasForeignKey(item => item.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InventoryItemRecord>(entity =>
        {
            entity.ToTable("InventoryItems");
            entity.HasKey(record => new
            {
                record.PlayerId,
                record.ItemId
            });
        });

        modelBuilder.Entity<RecipeRecord>(entity =>
        {
            entity.ToTable("Recipes");
            entity.HasKey(record => record.RecipeId);

            entity.HasMany(record => record.Ingredients)
                .WithOne(ingredient => ingredient.Recipe)
                .HasForeignKey(ingredient => ingredient.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RecipeIngredientRecord>(entity =>
        {
            entity.ToTable("RecipeIngredients");
            entity.HasKey(record => new
            {
                record.RecipeId,
                record.Position
            });
        });

        modelBuilder.Entity<FoodRecord>(entity =>
        {
            entity.ToTable("Foods");
            entity.HasKey(record => record.FoodId);
        });
    }
}
