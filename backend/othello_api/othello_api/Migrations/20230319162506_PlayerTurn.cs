using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace othelloapi.Migrations
{
    /// <inheritdoc />
    public partial class PlayerTurn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlayerTurn",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerTurn",
                table: "Games");
        }
    }
}
