using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _19042024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DisabledTill",
                table: "Chats",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisabledTill",
                table: "Chats");
        }
    }
}
