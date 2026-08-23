using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookAndBookOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Author",
                table: "Books",
                newName: "AuthorNames");

            migrationBuilder.AlterColumn<string>(
                name: "CustomAuthor",
                table: "BookOwners",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuthorNames",
                table: "Books",
                newName: "Author");

            migrationBuilder.AlterColumn<string>(
                name: "CustomAuthor",
                table: "BookOwners",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
