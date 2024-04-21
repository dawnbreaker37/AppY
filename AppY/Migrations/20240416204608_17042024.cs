using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _17042024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Chats",
                type: "nvarchar(600)",
                maxLength: 600,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Shortname",
                table: "Chats",
                type: "nvarchar(24)",
                maxLength: 24,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "Shortname",
                table: "Chats");
        }
    }
}
