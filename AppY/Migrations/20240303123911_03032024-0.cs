using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _030320240 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NotificationCategoryId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "NotificationCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NotificationCategoryId",
                table: "Notifications",
                column: "NotificationCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationCategories_NotificationCategoryId",
                table: "Notifications",
                column: "NotificationCategoryId",
                principalTable: "NotificationCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationCategories_NotificationCategoryId",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "NotificationCategories");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_NotificationCategoryId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NotificationCategoryId",
                table: "Notifications");
        }
    }
}
