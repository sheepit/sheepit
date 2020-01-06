using Microsoft.EntityFrameworkCore.Migrations;

namespace SheepIt.Api.Migrations
{
    public partial class foreignkeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Package_DeploymentProcessId",
                table: "Package",
                column: "DeploymentProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_Package_ProjectId",
                table: "Package",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Environment_ProjectId",
                table: "Environment",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DeploymentProcess_ProjectId",
                table: "DeploymentProcess",
                column: "ProjectId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Deployment_Environment_EnvironmentId",
                table: "Deployment",
                column: "EnvironmentId",
                principalTable: "Environment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Deployment_Package_PackageId",
                table: "Deployment",
                column: "PackageId",
                principalTable: "Package",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Deployment_Project_ProjectId",
                table: "Deployment",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeploymentProcess_Project_ProjectId",
                table: "DeploymentProcess",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Environment_Project_ProjectId",
                table: "Environment",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Package_DeploymentProcess_DeploymentProcessId",
                table: "Package",
                column: "DeploymentProcessId",
                principalTable: "DeploymentProcess",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Package_Project_ProjectId",
                table: "Package",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deployment_Environment_EnvironmentId",
                table: "Deployment");

            migrationBuilder.DropForeignKey(
                name: "FK_Deployment_Package_PackageId",
                table: "Deployment");

            migrationBuilder.DropForeignKey(
                name: "FK_Deployment_Project_ProjectId",
                table: "Deployment");

            migrationBuilder.DropForeignKey(
                name: "FK_DeploymentProcess_Project_ProjectId",
                table: "DeploymentProcess");

            migrationBuilder.DropForeignKey(
                name: "FK_Environment_Project_ProjectId",
                table: "Environment");

            migrationBuilder.DropForeignKey(
                name: "FK_Package_DeploymentProcess_DeploymentProcessId",
                table: "Package");

            migrationBuilder.DropForeignKey(
                name: "FK_Package_Project_ProjectId",
                table: "Package");

            migrationBuilder.DropIndex(
                name: "IX_Package_DeploymentProcessId",
                table: "Package");

            migrationBuilder.DropIndex(
                name: "IX_Package_ProjectId",
                table: "Package");

            migrationBuilder.DropIndex(
                name: "IX_Environment_ProjectId",
                table: "Environment");

            migrationBuilder.DropIndex(
                name: "IX_DeploymentProcess_ProjectId",
                table: "DeploymentProcess");

            migrationBuilder.DropIndex(
                name: "IX_Deployment_EnvironmentId",
                table: "Deployment");

            migrationBuilder.DropIndex(
                name: "IX_Deployment_PackageId",
                table: "Deployment");

            migrationBuilder.DropIndex(
                name: "IX_Deployment_ProjectId",
                table: "Deployment");
        }
    }
}
