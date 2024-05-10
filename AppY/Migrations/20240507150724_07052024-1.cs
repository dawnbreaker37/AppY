using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _070520241 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_SavedMessages_SavedMessageId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedMessagesContent_SavedMessages_SavedMessageId",
                table: "SavedMessagesContent");

            migrationBuilder.DropTable(
                name: "SavedMessages");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SavedMessageId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SavedMessageId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "SavedMessageId",
                table: "SavedMessagesContent",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_SavedMessagesContent_SavedMessageId",
                table: "SavedMessagesContent",
                newName: "IX_SavedMessagesContent_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedMessagesContent_AspNetUsers_UserId",
                table: "SavedMessagesContent",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedMessagesContent_AspNetUsers_UserId",
                table: "SavedMessagesContent");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "SavedMessagesContent",
                newName: "SavedMessageId");

            migrationBuilder.RenameIndex(
                name: "IX_SavedMessagesContent_UserId",
                table: "SavedMessagesContent",
                newName: "IX_SavedMessagesContent_SavedMessageId");

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

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SavedMessageId",
                table: "AspNetUsers",
                column: "SavedMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedMessages_UserId",
                table: "SavedMessages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_SavedMessages_SavedMessageId",
                table: "AspNetUsers",
                column: "SavedMessageId",
                principalTable: "SavedMessages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedMessagesContent_SavedMessages_SavedMessageId",
                table: "SavedMessagesContent",
                column: "SavedMessageId",
                principalTable: "SavedMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
