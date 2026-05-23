using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchReadLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddProgressFieldsToLibraryItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentProgress",
                table: "LibraryItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProgressType",
                table: "LibraryItems",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TotalProgress",
                table: "LibraryItems",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentProgress",
                table: "LibraryItems");

            migrationBuilder.DropColumn(
                name: "ProgressType",
                table: "LibraryItems");

            migrationBuilder.DropColumn(
                name: "TotalProgress",
                table: "LibraryItems");
        }
    }
}
