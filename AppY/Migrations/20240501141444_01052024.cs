using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _01052024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SecretChats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Encryption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitiatorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecretChats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecretChatUser",
                columns: table => new
                {
                    SecretChatsId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecretChatUser", x => new { x.SecretChatsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_SecretChatUser_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SecretChatUser_SecretChats_SecretChatsId",
                        column: x => x.SecretChatsId,
                        principalTable: "SecretChats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SecretChatUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SecretChatId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecretChatUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecretChatUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SecretChatUsers_SecretChats_SecretChatId",
                        column: x => x.SecretChatId,
                        principalTable: "SecretChats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SecretChatUser_UsersId",
                table: "SecretChatUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_SecretChatUsers_SecretChatId",
                table: "SecretChatUsers",
                column: "SecretChatId");

            migrationBuilder.CreateIndex(
                name: "IX_SecretChatUsers_UserId",
                table: "SecretChatUsers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SecretChatUser");

            migrationBuilder.DropTable(
                name: "SecretChatUsers");

            migrationBuilder.DropTable(
                name: "SecretChats");
        }
    }
}
