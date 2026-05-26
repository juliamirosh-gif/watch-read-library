using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchReadLibrary.Migrations
{
    /// <inheritdoc />
    public partial class ForceAddUserIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
        ALTER TABLE "LibraryItems"
        ADD COLUMN IF NOT EXISTS "UserId" text;
    """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
