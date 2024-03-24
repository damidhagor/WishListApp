using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WishListApp.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class NewNamingConvention : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WishlistItems_WishlistShares_BoughtByWishlistShareId",
                table: "WishlistItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WishlistItems_Wishlists_WishlistId",
                table: "WishlistItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WishlistShares_Wishlists_WishlistId",
                table: "WishlistShares");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WishlistShares",
                table: "WishlistShares");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WishlistItems",
                table: "WishlistItems");

            migrationBuilder.RenameTable(
                name: "WishlistShares",
                newName: "Shares");

            migrationBuilder.RenameTable(
                name: "WishlistItems",
                newName: "Items");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistShares_WishlistId",
                table: "Shares",
                newName: "IX_Shares_WishlistId");

            migrationBuilder.RenameColumn(
                name: "BoughtByWishlistShareId",
                table: "Items",
                newName: "BuyerShareId");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistItems_WishlistId",
                table: "Items",
                newName: "IX_Items_WishlistId");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistItems_BoughtByWishlistShareId",
                table: "Items",
                newName: "IX_Items_BuyerShareId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shares",
                table: "Shares",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Items",
                table: "Items",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Shares_BuyerShareId",
                table: "Items",
                column: "BuyerShareId",
                principalTable: "Shares",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Wishlists_WishlistId",
                table: "Items",
                column: "WishlistId",
                principalTable: "Wishlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shares_Wishlists_WishlistId",
                table: "Shares",
                column: "WishlistId",
                principalTable: "Wishlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Shares_BuyerShareId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Wishlists_WishlistId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Shares_Wishlists_WishlistId",
                table: "Shares");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shares",
                table: "Shares");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Items",
                table: "Items");

            migrationBuilder.RenameTable(
                name: "Shares",
                newName: "WishlistShares");

            migrationBuilder.RenameTable(
                name: "Items",
                newName: "WishlistItems");

            migrationBuilder.RenameIndex(
                name: "IX_Shares_WishlistId",
                table: "WishlistShares",
                newName: "IX_WishlistShares_WishlistId");

            migrationBuilder.RenameColumn(
                name: "BuyerShareId",
                table: "WishlistItems",
                newName: "BoughtByWishlistShareId");

            migrationBuilder.RenameIndex(
                name: "IX_Items_WishlistId",
                table: "WishlistItems",
                newName: "IX_WishlistItems_WishlistId");

            migrationBuilder.RenameIndex(
                name: "IX_Items_BuyerShareId",
                table: "WishlistItems",
                newName: "IX_WishlistItems_BoughtByWishlistShareId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WishlistShares",
                table: "WishlistShares",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WishlistItems",
                table: "WishlistItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistItems_WishlistShares_BoughtByWishlistShareId",
                table: "WishlistItems",
                column: "BoughtByWishlistShareId",
                principalTable: "WishlistShares",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistItems_Wishlists_WishlistId",
                table: "WishlistItems",
                column: "WishlistId",
                principalTable: "Wishlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistShares_Wishlists_WishlistId",
                table: "WishlistShares",
                column: "WishlistId",
                principalTable: "Wishlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
