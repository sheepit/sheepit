using Microsoft.EntityFrameworkCore.Migrations;

namespace SheepIt.Api.Migrations
{
    public partial class packageidsequence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "package");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "package");
        }
    }
}
