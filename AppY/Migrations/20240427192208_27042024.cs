using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _27042024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatSecondUserName",
                table: "ChatUsers");

            migrationBuilder.AddColumn<int>(
                name: "ChatUserId",
                table: "ChatMessages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ChatUserId",
                table: "ChatMessages",
                column: "ChatUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_ChatUsers_ChatUserId",
                table: "ChatMessages",
                column: "ChatUserId",
                principalTable: "ChatUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_ChatUsers_ChatUserId",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_ChatUserId",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "ChatUserId",
                table: "ChatMessages");

            migrationBuilder.AddColumn<string>(
                name: "ChatSecondUserName",
                table: "ChatUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
