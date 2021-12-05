using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPI.Data.Migrations
{
    public partial class CreatorGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorApplicationUserId",
                table: "Groups",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_CreatorApplicationUserId",
                table: "Groups",
                column: "CreatorApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_AspNetUsers_CreatorApplicationUserId",
                table: "Groups",
                column: "CreatorApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_AspNetUsers_CreatorApplicationUserId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_CreatorApplicationUserId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "CreatorApplicationUserId",
                table: "Groups");
        }
    }
}
