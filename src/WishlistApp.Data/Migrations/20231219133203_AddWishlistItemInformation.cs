using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WishlistApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWishlistItemInformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "WishlistItems",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "WishlistItems",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "WishlistItems",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Price",
                table: "WishlistItems",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "WishlistItems");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "WishlistItems");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "WishlistItems");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "WishlistItems");
        }
    }
}
