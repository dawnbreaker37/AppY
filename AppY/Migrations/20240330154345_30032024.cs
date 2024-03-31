using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _30032024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReactionCode = table.Column<string>(type: "nvarchar(90)", maxLength: 90, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "discussionMessageReactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReactionId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DiscussionMessageId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discussionMessageReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_discussionMessageReactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_discussionMessageReactions_DiscussionMessages_DiscussionMessageId",
                        column: x => x.DiscussionMessageId,
                        principalTable: "DiscussionMessages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_discussionMessageReactions_Reactions_ReactionId",
                        column: x => x.ReactionId,
                        principalTable: "Reactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_discussionMessageReactions_DiscussionMessageId",
                table: "discussionMessageReactions",
                column: "DiscussionMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_discussionMessageReactions_ReactionId",
                table: "discussionMessageReactions",
                column: "ReactionId");

            migrationBuilder.CreateIndex(
                name: "IX_discussionMessageReactions_UserId",
                table: "discussionMessageReactions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "discussionMessageReactions");

            migrationBuilder.DropTable(
                name: "Reactions");
        }
    }
}
