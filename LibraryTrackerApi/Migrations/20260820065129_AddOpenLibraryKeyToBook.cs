using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class AddOpenLibraryKeyToBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OpenLibraryKey",
                table: "Books",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpenLibraryKey",
                table: "Books");
        }
    }
}
