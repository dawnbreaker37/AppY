using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _08032024re : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionUsers_DiscussionUsers_DiscussionId",
                table: "DiscussionUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionUsers_Discussions_DiscussionId",
                table: "DiscussionUsers",
                column: "DiscussionId",
                principalTable: "Discussions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionUsers_Discussions_DiscussionId",
                table: "DiscussionUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionUsers_DiscussionUsers_DiscussionId",
                table: "DiscussionUsers",
                column: "DiscussionId",
                principalTable: "DiscussionUsers",
                principalColumn: "Id");
        }
    }
}
