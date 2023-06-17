using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace othelloapi.Migrations
{
    /// <inheritdoc />
    public partial class initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Blogs",
                table: "Blogs");

            migrationBuilder.RenameTable(
                name: "Blogs",
                newName: "Games");

            migrationBuilder.RenameColumn(
                name: "GameId",
                table: "Games",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "BlackTime",
                table: "Games",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Games",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Games",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeIncrement",
                table: "Games",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TimeLimit",
                table: "Games",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WhiteTime",
                table: "Games",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Games",
                table: "Games",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Games",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "BlackTime",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "TimeIncrement",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "TimeLimit",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "WhiteTime",
                table: "Games");

            migrationBuilder.RenameTable(
                name: "Games",
                newName: "Blogs");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Blogs",
                newName: "GameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Blogs",
                table: "Blogs",
                column: "GameId");
        }
    }
}
