using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _09032024re : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionMessage_AspNetUsers_UserId",
                table: "DiscussionMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionMessage_Discussions_DiscussionId",
                table: "DiscussionMessage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiscussionMessage",
                table: "DiscussionMessage");

            migrationBuilder.RenameTable(
                name: "DiscussionMessage",
                newName: "DiscussionMessages");

            migrationBuilder.RenameIndex(
                name: "IX_DiscussionMessage_UserId",
                table: "DiscussionMessages",
                newName: "IX_DiscussionMessages_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscussionMessage_DiscussionId",
                table: "DiscussionMessages",
                newName: "IX_DiscussionMessages_DiscussionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiscussionMessages",
                table: "DiscussionMessages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionMessages_AspNetUsers_UserId",
                table: "DiscussionMessages",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionMessages_Discussions_DiscussionId",
                table: "DiscussionMessages",
                column: "DiscussionId",
                principalTable: "Discussions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionMessages_AspNetUsers_UserId",
                table: "DiscussionMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionMessages_Discussions_DiscussionId",
                table: "DiscussionMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiscussionMessages",
                table: "DiscussionMessages");

            migrationBuilder.RenameTable(
                name: "DiscussionMessages",
                newName: "DiscussionMessage");

            migrationBuilder.RenameIndex(
                name: "IX_DiscussionMessages_UserId",
                table: "DiscussionMessage",
                newName: "IX_DiscussionMessage_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscussionMessages_DiscussionId",
                table: "DiscussionMessage",
                newName: "IX_DiscussionMessage_DiscussionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiscussionMessage",
                table: "DiscussionMessage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionMessage_AspNetUsers_UserId",
                table: "DiscussionMessage",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionMessage_Discussions_DiscussionId",
                table: "DiscussionMessage",
                column: "DiscussionId",
                principalTable: "Discussions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
