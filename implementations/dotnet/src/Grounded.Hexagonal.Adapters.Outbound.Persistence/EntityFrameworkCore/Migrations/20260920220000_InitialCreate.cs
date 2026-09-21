using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Grounded.Hexagonal.Adapters.Outbound.Persistence.EntityFrameworkCore.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Foods",
            columns: table => new
            {
                FoodId = table.Column<Guid>(
                    type: "TEXT",
                    nullable: false),
                SpoilsAtUtc = table.Column<DateTime>(
                    type: "TEXT",
                    nullable: false),
                State = table.Column<int>(
                    type: "INTEGER",
                    nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(
                    "PK_Foods",
                    x => x.FoodId);
            });

        migrationBuilder.CreateTable(
            name: "Inventories",
            columns: table => new
            {
                PlayerId = table.Column<Guid>(
                    type: "TEXT",
                    nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(
                    "PK_Inventories",
                    x => x.PlayerId);
            });

        migrationBuilder.CreateTable(
            name: "Recipes",
            columns: table => new
            {
                RecipeId = table.Column<Guid>(
                    type: "TEXT",
                    nullable: false),
                ResultItemId = table.Column<Guid>(
                    type: "TEXT",
                    nullable: false),
                ResultQuantity = table.Column<int>(
                    type: "INTEGER",
                    nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(
                    "PK_Recipes",
                    x => x.RecipeId);
            });

        migrationBuilder.CreateTable(
            name: "InventoryItems",
            columns: table => new
            {
                PlayerId = table.Column<Guid>(
                    type: "TEXT",
                    nullable: false),
                ItemId = table.Column<Guid>(
                    type: "TEXT",
                    nullable: false),
                Quantity = table.Column<int>(
                    type: "INTEGER",
                    nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(
                    "PK_InventoryItems",
                    x => new
                    {
                        x.PlayerId,
                        x.ItemId
                    });

                table.ForeignKey(
                    name: "FK_InventoryItems_Inventories_PlayerId",
                    column: x => x.PlayerId,
                    principalTable: "Inventories",
                    principalColumn: "PlayerId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "RecipeIngredients",
            columns: table => new
            {
                RecipeId = table.Column<Guid>(
                    type: "TEXT",
                    nullable: false),
                Position = table.Column<int>(
                    type: "INTEGER",
                    nullable: false),
                ItemId = table.Column<Guid>(
                    type: "TEXT",
                    nullable: false),
                Quantity = table.Column<int>(
                    type: "INTEGER",
                    nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(
                    "PK_RecipeIngredients",
                    x => new
                    {
                        x.RecipeId,
                        x.Position
                    });

                table.ForeignKey(
                    name: "FK_RecipeIngredients_Recipes_RecipeId",
                    column: x => x.RecipeId,
                    principalTable: "Recipes",
                    principalColumn: "RecipeId",
                    onDelete: ReferentialAction.Cascade);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Foods");

        migrationBuilder.DropTable(
            name: "InventoryItems");

        migrationBuilder.DropTable(
            name: "RecipeIngredients");

        migrationBuilder.DropTable(
            name: "Inventories");

        migrationBuilder.DropTable(
            name: "Recipes");
    }
}
