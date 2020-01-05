using System;
using Microsoft.EntityFrameworkCore.Migrations;
using SheepIt.Api.Core.Deployments;

namespace SheepIt.Api.Migrations
{
    public partial class tabledeployment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "deployment");

            migrationBuilder.CreateTable(
                name: "Deployment",
                columns: table => new
                {
                    ObjectId = table.Column<Guid>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    ProjectId = table.Column<string>(nullable: true),
                    PackageId = table.Column<int>(nullable: false),
                    StartedAt = table.Column<DateTime>(nullable: false),
                    EnvironmentId = table.Column<int>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    ProcessOutput = table.Column<ProcessOutput>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deployment", x => x.ObjectId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Deployment");

            migrationBuilder.DropSequence(
                name: "deployment");
        }
    }
}
