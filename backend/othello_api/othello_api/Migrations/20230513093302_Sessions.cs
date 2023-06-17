using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace othelloapi.Migrations
{
    /// <inheritdoc />
    public partial class Sessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GameId",
                table: "ChatMessages",
                newName: "SesssionId");

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "SesssionId",
                table: "ChatMessages",
                newName: "GameId");
        }
    }
}
