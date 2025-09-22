using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertySellingApp.DataAccess.Migrations
{
    public partial class Addimagefield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Properties");
        }
    }
}
