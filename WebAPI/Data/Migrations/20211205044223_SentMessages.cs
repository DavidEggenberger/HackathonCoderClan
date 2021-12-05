using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPI.Data.Migrations
{
    public partial class SentMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Message",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentTime",
                table: "Message",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Message_ApplicationUserId",
                table: "Message",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_AspNetUsers_ApplicationUserId",
                table: "Message",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_AspNetUsers_ApplicationUserId",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_ApplicationUserId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "SentTime",
                table: "Message");
        }
    }
}
