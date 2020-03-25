using Microsoft.EntityFrameworkCore.Migrations;

namespace SheepIt.Api.Migrations
{
    public partial class packagesarenowrelatedtocomponents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ComponentId",
                table: "Package",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Package_ComponentId",
                table: "Package",
                column: "ComponentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Package_Components_ComponentId",
                table: "Package",
                column: "ComponentId",
                principalTable: "Components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Package_Components_ComponentId",
                table: "Package");

            migrationBuilder.DropIndex(
                name: "IX_Package_ComponentId",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "ComponentId",
                table: "Package");
        }
    }
}
