using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangeGenreToSubjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Genre",
                table: "Books",
                newName: "Subjects");

            migrationBuilder.AddColumn<string>(
                name: "CustomGenre",
                table: "BookOwners",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomGenre",
                table: "BookOwners");

            migrationBuilder.RenameColumn(
                name: "Subjects",
                table: "Books",
                newName: "Genre");
        }
    }
}
