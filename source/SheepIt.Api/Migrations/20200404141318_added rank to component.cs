using Microsoft.EntityFrameworkCore.Migrations;

namespace SheepIt.Api.Migrations
{
    public partial class addedranktocomponent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Components",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Components");
        }
    }
}
