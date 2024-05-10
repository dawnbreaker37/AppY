using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _07052024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SavedMessageId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SavedMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedMessages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedMessagesContent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Badge = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(900)", maxLength: 900, nullable: true),
                    ChatMessageId = table.Column<int>(type: "int", nullable: true),
                    DiscussionMessageId = table.Column<int>(type: "int", nullable: true),
                    SavedMessageId = table.Column<int>(type: "int", nullable: false),
                    IsEdited = table.Column<bool>(type: "bit", nullable: false),
                    IsPinned = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedMessagesContent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedMessagesContent_ChatMessages_ChatMessageId",
                        column: x => x.ChatMessageId,
                        principalTable: "ChatMessages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SavedMessagesContent_DiscussionMessages_DiscussionMessageId",
                        column: x => x.DiscussionMessageId,
                        principalTable: "DiscussionMessages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SavedMessagesContent_SavedMessages_SavedMessageId",
                        column: x => x.SavedMessageId,
                        principalTable: "SavedMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SavedMessageId",
                table: "AspNetUsers",
                column: "SavedMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedMessages_UserId",
                table: "SavedMessages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedMessagesContent_ChatMessageId",
                table: "SavedMessagesContent",
                column: "ChatMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedMessagesContent_DiscussionMessageId",
                table: "SavedMessagesContent",
                column: "DiscussionMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedMessagesContent_SavedMessageId",
                table: "SavedMessagesContent",
                column: "SavedMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_SavedMessages_SavedMessageId",
                table: "AspNetUsers",
                column: "SavedMessageId",
                principalTable: "SavedMessages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_SavedMessages_SavedMessageId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "SavedMessagesContent");

            migrationBuilder.DropTable(
                name: "SavedMessages");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SavedMessageId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SavedMessageId",
                table: "AspNetUsers");
        }
    }
}
