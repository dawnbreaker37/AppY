using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _230320241 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionMessages_DiscussionMessages_DiscussionMessageId",
                table: "DiscussionMessages");

            migrationBuilder.DropIndex(
                name: "IX_DiscussionMessages_DiscussionMessageId",
                table: "DiscussionMessages");

            migrationBuilder.DropColumn(
                name: "DiscussionMessageId",
                table: "DiscussionMessages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscussionMessageId",
                table: "DiscussionMessages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionMessages_DiscussionMessageId",
                table: "DiscussionMessages",
                column: "DiscussionMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionMessages_DiscussionMessages_DiscussionMessageId",
                table: "DiscussionMessages",
                column: "DiscussionMessageId",
                principalTable: "DiscussionMessages",
                principalColumn: "Id");
        }
    }
}
