using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchReadLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddYearToLibraryItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "LibraryItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Year",
                table: "LibraryItems");
        }
    }
}
