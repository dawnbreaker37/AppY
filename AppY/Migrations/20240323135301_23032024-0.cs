using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _230320240 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscussionId",
                table: "DiscussionMessageReplies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionMessageReplies_DiscussionId",
                table: "DiscussionMessageReplies",
                column: "DiscussionId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionMessageReplies_Discussions_DiscussionId",
                table: "DiscussionMessageReplies",
                column: "DiscussionId",
                principalTable: "Discussions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionMessageReplies_Discussions_DiscussionId",
                table: "DiscussionMessageReplies");

            migrationBuilder.DropIndex(
                name: "IX_DiscussionMessageReplies_DiscussionId",
                table: "DiscussionMessageReplies");

            migrationBuilder.DropColumn(
                name: "DiscussionId",
                table: "DiscussionMessageReplies");
        }
    }
}
