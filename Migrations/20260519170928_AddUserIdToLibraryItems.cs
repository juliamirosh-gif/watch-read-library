using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchReadLibrary.Migrations
{
    public partial class AddUserIdToLibraryItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "LibraryItems",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "LibraryItems");
        }
    }
}