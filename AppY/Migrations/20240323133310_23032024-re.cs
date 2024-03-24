using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _23032024re : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionMessageReplies_AspNetUsers_UserId",
                table: "DiscussionMessageReplies");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "DiscussionMessageReplies",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionMessageReplies_AspNetUsers_UserId",
                table: "DiscussionMessageReplies",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionMessageReplies_AspNetUsers_UserId",
                table: "DiscussionMessageReplies");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "DiscussionMessageReplies",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionMessageReplies_AspNetUsers_UserId",
                table: "DiscussionMessageReplies",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
