using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _22042024re : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinkedAccounts_AspNetUsers_UserId",
                table: "LinkedAccounts");

            migrationBuilder.DropIndex(
                name: "IX_LinkedAccounts_UserId",
                table: "LinkedAccounts");

            migrationBuilder.CreateIndex(
                name: "IX_LinkedAccounts_LinkedUserId",
                table: "LinkedAccounts",
                column: "LinkedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkedAccounts_AspNetUsers_LinkedUserId",
                table: "LinkedAccounts",
                column: "LinkedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinkedAccounts_AspNetUsers_LinkedUserId",
                table: "LinkedAccounts");

            migrationBuilder.DropIndex(
                name: "IX_LinkedAccounts_LinkedUserId",
                table: "LinkedAccounts");

            migrationBuilder.CreateIndex(
                name: "IX_LinkedAccounts_UserId",
                table: "LinkedAccounts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkedAccounts_AspNetUsers_UserId",
                table: "LinkedAccounts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
