using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace othelloapi.Migrations
{
    /// <inheritdoc />
    public partial class addedMoveNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameMoves_Games_GameId",
                table: "GameMoves");

            migrationBuilder.DropIndex(
                name: "IX_GameMoves_GameId",
                table: "GameMoves");

            migrationBuilder.RenameColumn(
                name: "PlayerTurn",
                table: "GameMoves",
                newName: "PlayerColor");

            migrationBuilder.AddColumn<int>(
                name: "MoveNumber",
                table: "GameMoves",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoveNumber",
                table: "GameMoves");

            migrationBuilder.RenameColumn(
                name: "PlayerColor",
                table: "GameMoves",
                newName: "PlayerTurn");

            migrationBuilder.CreateIndex(
                name: "IX_GameMoves_GameId",
                table: "GameMoves",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameMoves_Games_GameId",
                table: "GameMoves",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
