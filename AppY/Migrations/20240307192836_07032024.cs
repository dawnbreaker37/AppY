using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _07032024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Discussions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discussions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscussionUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    DiscussionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscussionUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscussionUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DiscussionUsers_DiscussionUsers_DiscussionId",
                        column: x => x.DiscussionId,
                        principalTable: "DiscussionUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DiscussionUser",
                columns: table => new
                {
                    DiscussionsId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscussionUser", x => new { x.DiscussionsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_DiscussionUser_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscussionUser_Discussions_DiscussionsId",
                        column: x => x.DiscussionsId,
                        principalTable: "Discussions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionUser_UsersId",
                table: "DiscussionUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionUsers_DiscussionId",
                table: "DiscussionUsers",
                column: "DiscussionId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionUsers_UserId",
                table: "DiscussionUsers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscussionUser");

            migrationBuilder.DropTable(
                name: "DiscussionUsers");

            migrationBuilder.DropTable(
                name: "Discussions");
        }
    }
}
