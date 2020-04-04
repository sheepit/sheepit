using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Model.Deployments;
using SheepIt.Api.Tests.FeatureObjects;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.UseCases.Dashboard;
using SheepIt.Api.UseCases.ProjectOperations.Deployments;

namespace SheepIt.Api.Tests.UseCases.Dashboard
{
    public class GetDashboardTests : Test<IntegrationTestsFixture>
    {
        [Test]
        public async Task can_get_deployments()
        {
            // given
            
            var project = await Fixture.CreateProject()
                .Create();

            Fixture.MomentLater();

            var firstPackage = await Fixture.CreatePackage(project.Id, project.FirstComponent.Id)
                .Create();
            
            Fixture.MomentLater();

            var secondPackage = await Fixture.CreatePackage(project.Id, project.FirstComponent.Id)
                .Create();
            
            Fixture.MomentLater();

            var firstDeploymentTime = Fixture.GetUtcNow();

            var firstDeployment = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = project.Id,
                PackageId = firstPackage.Id,
                EnvironmentId = project.FirstEnvironment.Id
            });

            Fixture.MomentLater();

            var secondDeploymentTime = Fixture.GetUtcNow();

            var secondDeployment = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = project.Id,
                PackageId = secondPackage.Id,
                EnvironmentId = project.SecondEnvironment.Id
            });

            // when

            var response = await Fixture.Handle(new GetDashboardRequest());
            
            // then
            
            response.LastDeployments.Should().BeEquivalentTo(new[]
            {
                new GetDashboardResponse.DeploymentDto
                {
                    ProjectId = project.Id,
                    DeploymentId = firstDeployment.CreatedDeploymentId,
                    Status = DeploymentStatus.Succeeded.ToString(),
                    DeployedAt = firstDeploymentTime,
                    EnvironmentId = project.FirstEnvironment.Id,
                    EnvironmentDisplayName = project.FirstEnvironment.Name
                },
                new GetDashboardResponse.DeploymentDto
                {
                    ProjectId = project.Id,
                    DeploymentId = secondDeployment.CreatedDeploymentId,
                    Status = DeploymentStatus.Succeeded.ToString(),
                    DeployedAt = secondDeploymentTime,
                    EnvironmentId = project.SecondEnvironment.Id,
                    EnvironmentDisplayName = project.SecondEnvironment.Name
                }
            });
        }
    }
}