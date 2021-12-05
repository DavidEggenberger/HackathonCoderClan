using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPI.Data.Migrations
{
    public partial class PrivateProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PrivateProfile",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrivateProfile",
                table: "AspNetUsers");
        }
    }
}
