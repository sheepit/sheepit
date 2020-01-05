using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SheepIt.Api.Migrations
{
    public partial class tabledeploymentprocess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "deploymentprocess");

            migrationBuilder.CreateTable(
                name: "DeploymentProcess",
                columns: table => new
                {
                    ObjectId = table.Column<Guid>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    ProjectId = table.Column<string>(nullable: true),
                    File = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeploymentProcess", x => x.ObjectId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeploymentProcess");

            migrationBuilder.DropSequence(
                name: "deploymentprocess");
        }
    }
}
