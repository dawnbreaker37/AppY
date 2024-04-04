using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _03042024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_discussionMessageReactions_AspNetUsers_UserId",
                table: "discussionMessageReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_discussionMessageReactions_DiscussionMessages_DiscussionMessageId",
                table: "discussionMessageReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_discussionMessageReactions_Reactions_ReactionId",
                table: "discussionMessageReactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_discussionMessageReactions",
                table: "discussionMessageReactions");

            migrationBuilder.RenameTable(
                name: "discussionMessageReactions",
                newName: "DiscussionMessageReactions");

            migrationBuilder.RenameIndex(
                name: "IX_discussionMessageReactions_UserId",
                table: "DiscussionMessageReactions",
                newName: "IX_DiscussionMessageReactions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_discussionMessageReactions_ReactionId",
                table: "DiscussionMessageReactions",
                newName: "IX_DiscussionMessageReactions_ReactionId");

            migrationBuilder.RenameIndex(
                name: "IX_discussionMessageReactions_DiscussionMessageId",
                table: "DiscussionMessageReactions",
                newName: "IX_DiscussionMessageReactions_DiscussionMessageId");

            migrationBuilder.AddColumn<bool>(
                name: "AreMessagesAutoDeletable",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSeenAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiscussionMessageReactions",
                table: "DiscussionMessageReactions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionMessageReactions_AspNetUsers_UserId",
                table: "DiscussionMessageReactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionMessageReactions_DiscussionMessages_DiscussionMessageId",
                table: "DiscussionMessageReactions",
                column: "DiscussionMessageId",
                principalTable: "DiscussionMessages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionMessageReactions_Reactions_ReactionId",
                table: "DiscussionMessageReactions",
                column: "ReactionId",
                principalTable: "Reactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionMessageReactions_AspNetUsers_UserId",
                table: "DiscussionMessageReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionMessageReactions_DiscussionMessages_DiscussionMessageId",
                table: "DiscussionMessageReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionMessageReactions_Reactions_ReactionId",
                table: "DiscussionMessageReactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiscussionMessageReactions",
                table: "DiscussionMessageReactions");

            migrationBuilder.DropColumn(
                name: "AreMessagesAutoDeletable",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastSeenAt",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "DiscussionMessageReactions",
                newName: "discussionMessageReactions");

            migrationBuilder.RenameIndex(
                name: "IX_DiscussionMessageReactions_UserId",
                table: "discussionMessageReactions",
                newName: "IX_discussionMessageReactions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscussionMessageReactions_ReactionId",
                table: "discussionMessageReactions",
                newName: "IX_discussionMessageReactions_ReactionId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscussionMessageReactions_DiscussionMessageId",
                table: "discussionMessageReactions",
                newName: "IX_discussionMessageReactions_DiscussionMessageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_discussionMessageReactions",
                table: "discussionMessageReactions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_discussionMessageReactions_AspNetUsers_UserId",
                table: "discussionMessageReactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_discussionMessageReactions_DiscussionMessages_DiscussionMessageId",
                table: "discussionMessageReactions",
                column: "DiscussionMessageId",
                principalTable: "DiscussionMessages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_discussionMessageReactions_Reactions_ReactionId",
                table: "discussionMessageReactions",
                column: "ReactionId",
                principalTable: "Reactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
