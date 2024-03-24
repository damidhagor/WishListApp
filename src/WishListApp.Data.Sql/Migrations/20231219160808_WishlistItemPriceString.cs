using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WishlistApp.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class WishlistItemPriceString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Price",
                table: "WishlistItems",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Price",
                table: "WishlistItems",
                type: "real",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);
        }
    }
}
