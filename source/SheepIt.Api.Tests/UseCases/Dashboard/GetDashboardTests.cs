using System;
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
        private string _projectId;

        [SetUp]
        public async Task set_up()
        {
            _projectId = "foo";

            await Fixture.CreateProject(_projectId)
                .WithEnvironmentNames("dev", "test", "prod")
                .Create();
            
            Fixture.MomentLater();
        }
        
        [Test]
        public async Task can_get_deployments()
        {
            // given

            var firstPackageDescription = "first package";
            
            var firstPackage = await Fixture.CreatePackage(_projectId)
                .WithDescription(firstPackageDescription)
                .Create();
            
            Fixture.MomentLater();

            var secondPackageDescription = "second package";
            
            var secondPackage = await Fixture.CreatePackage(_projectId)
                .WithDescription(secondPackageDescription)
                .Create();
            
            Fixture.MomentLater();

            var firstDeploymentTime = Fixture.GetUtcNow();

            var firstDeployment = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = _projectId,
                PackageId = firstPackage.CreatedPackageId,
                EnvironmentId = 1
            });

            Fixture.MomentLater();

            var secondDeploymentTime = Fixture.GetUtcNow();

            var secondDeployment = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = _projectId,
                PackageId = secondPackage.CreatedPackageId,
                EnvironmentId = 2
            });

            // when

            var response = await Fixture.Handle(new GetDashboardRequest());
            
            // then
            
            response.LastDeployments.Should().BeEquivalentTo(new[]
            {
                new GetDashboardResponse.DeploymentDto
                {
                    ProjectId = _projectId,
                    DeploymentId = firstDeployment.CreatedDeploymentId,
                    Status = DeploymentStatus.Succeeded.ToString(),
                    DeployedAt = firstDeploymentTime,
                    EnvironmentId = 1,
                    EnvironmentDisplayName = "dev"
                },
                new GetDashboardResponse.DeploymentDto
                {
                    ProjectId = _projectId,
                    DeploymentId = secondDeployment.CreatedDeploymentId,
                    Status = DeploymentStatus.Succeeded.ToString(),
                    DeployedAt = secondDeploymentTime,
                    EnvironmentId = 2,
                    EnvironmentDisplayName = "test"
                }
            });
        }
    }
}