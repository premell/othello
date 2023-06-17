using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace othelloapi.Migrations
{
    /// <inheritdoc />
    public partial class removedTimer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlackTimeSeconds",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "WhiteTimeSeconds",
                table: "Games");

            migrationBuilder.AddColumn<DateTime>(
                name: "GameStarttime",
                table: "Games",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameStarttime",
                table: "Games");

            migrationBuilder.AddColumn<int>(
                name: "BlackTimeSeconds",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WhiteTimeSeconds",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
