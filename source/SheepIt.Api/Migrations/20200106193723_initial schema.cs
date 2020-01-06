using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Packages;

namespace SheepIt.Api.Migrations
{
    public partial class initialschema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "deployment");

            migrationBuilder.CreateSequence<int>(
                name: "deploymentprocess");

            migrationBuilder.CreateSequence<int>(
                name: "environment");

            migrationBuilder.CreateSequence<int>(
                name: "package");

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeploymentProcess",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<string>(nullable: false),
                    File = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeploymentProcess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeploymentProcess_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Environment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<string>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    Rank = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Environment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Environment_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Package",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<string>(nullable: false),
                    DeploymentProcessId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Variables = table.Column<VariableCollection>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Package", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Package_DeploymentProcess_DeploymentProcessId",
                        column: x => x.DeploymentProcessId,
                        principalTable: "DeploymentProcess",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Package_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Deployment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<string>(nullable: false),
                    PackageId = table.Column<int>(nullable: false),
                    EnvironmentId = table.Column<int>(nullable: false),
                    StartedAt = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    ProcessOutput = table.Column<ProcessOutput>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deployment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deployment_Environment_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalTable: "Environment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Deployment_Package_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Package",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Deployment_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deployment_EnvironmentId",
                table: "Deployment",
                column: "EnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Deployment_PackageId",
                table: "Deployment",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Deployment_ProjectId",
                table: "Deployment",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DeploymentProcess_ProjectId",
                table: "DeploymentProcess",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Environment_ProjectId",
                table: "Environment",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Package_DeploymentProcessId",
                table: "Package",
                column: "DeploymentProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_Package_ProjectId",
                table: "Package",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Deployment");

            migrationBuilder.DropTable(
                name: "Environment");

            migrationBuilder.DropTable(
                name: "Package");

            migrationBuilder.DropTable(
                name: "DeploymentProcess");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropSequence(
                name: "deployment");

            migrationBuilder.DropSequence(
                name: "deploymentprocess");

            migrationBuilder.DropSequence(
                name: "environment");

            migrationBuilder.DropSequence(
                name: "package");
        }
    }
}
