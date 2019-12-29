using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SheepIt.Api.Migrations
{
    public partial class tableenvironment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "environment");

            migrationBuilder.CreateTable(
                name: "Environment",
                columns: table => new
                {
                    ObjectId = table.Column<Guid>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    ProjectId = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    Rank = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Environment", x => x.ObjectId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Environment");

            migrationBuilder.DropSequence(
                name: "environment");
        }
    }
}
