using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Tests.FeatureObjects;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectOperations.Dashboard;
using SheepIt.Api.UseCases.ProjectOperations.Deployments;

namespace SheepIt.Api.Tests.UseCases.ProjectOperations.Dashboard
{
    public class GetProjectDashboardTests : Test<IntegrationTestsFixture>
    {
        private string _projectId;
        private DateTime _projectCreationTime;

        [SetUp]
        public async Task set_up()
        {
            _projectId = "foo";

            _projectCreationTime = Fixture.GetUtcNow();

            await Fixture.CreateProject(_projectId)
                .WithEnvironmentNames("dev", "test", "prod")
                .Create();
            
            Fixture.MomentLater();
        }

        [Test]
        public async Task can_get_environments()
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

            var response = await Fixture.Handle(new GetProjectDashboardRequest
            {
                ProjectId = _projectId
            });
            
            // then

            response.Environments.Should().BeEquivalentTo(new[]
            {
                new GetProjectDashboardResponse.EnvironmentDto
                {
                    EnvironmentId = 1,
                    DisplayName = "dev",
                    Deployment = new GetProjectDashboardResponse.EnvironmentDeploymentDto
                    {
                        CurrentDeploymentId = firstDeployment.CreatedDeploymentId,
                        CurrentPackageId = firstPackage.CreatedPackageId, 
                        CurrentPackageDescription = firstPackageDescription,
                        LastDeployedAt = firstDeploymentTime
                    }
                },
                new GetProjectDashboardResponse.EnvironmentDto
                {
                    EnvironmentId = 2,
                    DisplayName = "test",
                    Deployment = new GetProjectDashboardResponse.EnvironmentDeploymentDto
                    {
                        CurrentDeploymentId = secondDeployment.CreatedDeploymentId,
                        CurrentPackageId = secondPackage.CreatedPackageId, 
                        CurrentPackageDescription = secondPackageDescription,
                        LastDeployedAt = secondDeploymentTime
                    }
                },
                new GetProjectDashboardResponse.EnvironmentDto
                {
                    EnvironmentId = 3,
                    DisplayName = "prod",
                    Deployment = null
                }
            });
        }
    }
}