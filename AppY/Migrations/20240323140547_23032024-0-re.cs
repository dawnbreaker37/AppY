using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _230320240re : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscussionMessageReplies");

            migrationBuilder.AddColumn<int>(
                name: "DiscussionMessageId",
                table: "DiscussionMessages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RepliedMessageId",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "RepliedMessageId",
                table: "DiscussionMessages");

            migrationBuilder.CreateTable(
                name: "DiscussionMessageReplies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiscussionId = table.Column<int>(type: "int", nullable: true),
                    MessageId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscussionMessageReplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscussionMessageReplies_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DiscussionMessageReplies_DiscussionMessages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "DiscussionMessages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DiscussionMessageReplies_Discussions_DiscussionId",
                        column: x => x.DiscussionId,
                        principalTable: "Discussions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionMessageReplies_DiscussionId",
                table: "DiscussionMessageReplies",
                column: "DiscussionId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionMessageReplies_MessageId",
                table: "DiscussionMessageReplies",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionMessageReplies_UserId",
                table: "DiscussionMessageReplies",
                column: "UserId");
        }
    }
}
