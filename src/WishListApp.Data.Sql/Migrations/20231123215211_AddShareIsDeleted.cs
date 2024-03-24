using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WishListApp.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddShareIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WishlistShares",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WishlistShares");
        }
    }
}
