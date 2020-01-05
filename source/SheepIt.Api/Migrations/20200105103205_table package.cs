using System;
using Microsoft.EntityFrameworkCore.Migrations;
using SheepIt.Api.Core.Packages;

namespace SheepIt.Api.Migrations
{
    public partial class tablepackage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Package",
                columns: table => new
                {
                    ObjectId = table.Column<Guid>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    ProjectId = table.Column<string>(nullable: true),
                    DeploymentProcessId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Variables = table.Column<VariableCollection>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Package", x => x.ObjectId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Package");
        }
    }
}
