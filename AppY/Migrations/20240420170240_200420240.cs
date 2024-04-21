using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _200420240 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Chats");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ChatUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ChatUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Chats",
                type: "datetime2",
                nullable: true);
        }
    }
}
