using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Tests.IntegrationTests.FeatureObjects;
using SheepIt.Api.Tests.IntegrationTests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectOperations.Deployments;
using SheepIt.Api.UseCases.ProjectOperations.Environments;

namespace SheepIt.Api.Tests.IntegrationTests.UseCases.ProjectOperations.Environments
{
    public class GetEnvironmentsListTests : Test<IntegrationTestsFixture>
    {
        [Test]
        public async Task can_get_environments()
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

            var response = await Fixture.Handle(new GetEnvironmentsListRequest
            {
                ProjectId = project.Id
            });
            
            // then

            response.Environments.Should().BeEquivalentTo(new[]
            {
                new GetEnvironmentsListResponse.EnvironmentDto
                {
                    EnvironmentId = project.FirstEnvironment.Id,
                    DisplayName = project.FirstEnvironment.Name,
                    Deployment = new GetEnvironmentsListResponse.EnvironmentDeploymentDto
                    {
                        CurrentDeploymentId = firstDeployment.CreatedDeploymentId,
                        CurrentPackageId = firstPackage.Id, 
                        CurrentPackageDescription = firstPackage.Description,
                        LastDeployedAt = firstDeploymentTime
                    }
                },
                new GetEnvironmentsListResponse.EnvironmentDto
                {
                    EnvironmentId = project.SecondEnvironment.Id,
                    DisplayName = project.SecondEnvironment.Name,
                    Deployment = new GetEnvironmentsListResponse.EnvironmentDeploymentDto
                    {
                        CurrentDeploymentId = secondDeployment.CreatedDeploymentId,
                        CurrentPackageId = secondPackage.Id, 
                        CurrentPackageDescription = secondPackage.Description,
                        LastDeployedAt = secondDeploymentTime
                    }
                },
                new GetEnvironmentsListResponse.EnvironmentDto
                {
                    EnvironmentId = project.ThirdEnvironment.Id,
                    DisplayName = project.ThirdEnvironment.Name,
                    Deployment = null
                }
            });
        }
    }
}