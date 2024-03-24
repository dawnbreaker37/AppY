using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppY.Migrations
{
    /// <inheritdoc />
    public partial class _23032024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEdited",
                table: "DiscussionMessageReplies");

            migrationBuilder.DropColumn(
                name: "ReactionId",
                table: "DiscussionMessageReplies");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "DiscussionMessageReplies");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "DiscussionMessageReplies");

            migrationBuilder.AddColumn<bool>(
                name: "IsReply",
                table: "DiscussionMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReply",
                table: "DiscussionMessages");

            migrationBuilder.AddColumn<bool>(
                name: "IsEdited",
                table: "DiscussionMessageReplies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReactionId",
                table: "DiscussionMessageReplies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                table: "DiscussionMessageReplies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "DiscussionMessageReplies",
                type: "nvarchar(2400)",
                maxLength: 2400,
                nullable: true);
        }
    }
}
