using Microsoft.EntityFrameworkCore.Migrations;

namespace SheepIt.Api.Migrations
{
    public partial class deploymentprocessisnowrelatedwithcomponentnotproject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeploymentProcess_Project_ProjectId",
                table: "DeploymentProcess");

            migrationBuilder.DropIndex(
                name: "IX_DeploymentProcess_ProjectId",
                table: "DeploymentProcess");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "DeploymentProcess");

            migrationBuilder.AddColumn<int>(
                name: "ComponentId",
                table: "DeploymentProcess",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DeploymentProcess_ComponentId",
                table: "DeploymentProcess",
                column: "ComponentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeploymentProcess_Components_ComponentId",
                table: "DeploymentProcess",
                column: "ComponentId",
                principalTable: "Components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeploymentProcess_Components_ComponentId",
                table: "DeploymentProcess");

            migrationBuilder.DropIndex(
                name: "IX_DeploymentProcess_ComponentId",
                table: "DeploymentProcess");

            migrationBuilder.DropColumn(
                name: "ComponentId",
                table: "DeploymentProcess");

            migrationBuilder.AddColumn<string>(
                name: "ProjectId",
                table: "DeploymentProcess",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_DeploymentProcess_ProjectId",
                table: "DeploymentProcess",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeploymentProcess_Project_ProjectId",
                table: "DeploymentProcess",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
