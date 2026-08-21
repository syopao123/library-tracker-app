using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomPropertiesToBookOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomAuthor",
                table: "BookOwners",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomImageUrl",
                table: "BookOwners",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomTitle",
                table: "BookOwners",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomAuthor",
                table: "BookOwners");

            migrationBuilder.DropColumn(
                name: "CustomImageUrl",
                table: "BookOwners");

            migrationBuilder.DropColumn(
                name: "CustomTitle",
                table: "BookOwners");
        }
    }
}
