using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace othelloapi.Migrations
{
    /// <inheritdoc />
    public partial class increasedSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GameMoves",
                table: "GameMoves");

            migrationBuilder.DropIndex(
                name: "IX_GameMoves_GameId",
                table: "GameMoves");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameMoves",
                table: "GameMoves",
                columns: new[] { "GameId", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GameMoves",
                table: "GameMoves");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameMoves",
                table: "GameMoves",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GameMoves_GameId",
                table: "GameMoves",
                column: "GameId");
        }
    }
}
