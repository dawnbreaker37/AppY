using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _230320241re : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RepliesMsgShortText",
                table: "DiscussionMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DiscussionMessageAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MessageId = table.Column<int>(type: "int", nullable: true),
                    IsEdited = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscussionMessageAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscussionMessageAnswers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DiscussionMessageAnswers_DiscussionMessages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "DiscussionMessages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionMessageAnswers_MessageId",
                table: "DiscussionMessageAnswers",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionMessageAnswers_UserId",
                table: "DiscussionMessageAnswers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscussionMessageAnswers");

            migrationBuilder.DropColumn(
                name: "RepliesMsgShortText",
                table: "DiscussionMessages");
        }
    }
}
