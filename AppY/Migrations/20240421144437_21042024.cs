using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _21042024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChatSecondUserName",
                table: "ChatUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMuted",
                table: "ChatUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatSecondUserName",
                table: "ChatUsers");

            migrationBuilder.DropColumn(
                name: "IsMuted",
                table: "ChatUsers");
        }
    }
}
