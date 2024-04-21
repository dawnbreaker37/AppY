using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _20042024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DisabledTill",
                table: "Chats",
                newName: "DeletedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "Chats",
                newName: "DisabledTill");
        }
    }
}
