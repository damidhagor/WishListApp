using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WishlistApp.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddWishlistOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerIdentifier",
                table: "Wishlists",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerIdentifier",
                table: "Wishlists");
        }
    }
}
