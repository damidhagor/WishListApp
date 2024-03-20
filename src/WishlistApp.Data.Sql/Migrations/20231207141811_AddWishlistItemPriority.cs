using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WishlistApp.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddWishlistItemPriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "WishlistItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "WishlistItems");
        }
    }
}
