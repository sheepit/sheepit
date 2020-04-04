using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Model.Deployments;
using SheepIt.Api.Tests.FeatureObjects;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectOperations.Dashboard;
using SheepIt.Api.UseCases.ProjectOperations.Deployments;

namespace SheepIt.Api.Tests.UseCases.ProjectOperations.Deployments
{
    public class GetDeploymentsListHandlerTests : Test<IntegrationTestsFixture>
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

            var response = await Fixture.Handle(new GetDeploymentsListRequest
            {
                ProjectId = project.Id
            });
            
            // then

            response.Deployments.Should().BeEquivalentTo(new[]
            {
                new GetDeploymentsListResponse.DeploymentDto
                {
                    Id = firstDeployment.CreatedDeploymentId,
                    Status = DeploymentStatus.Succeeded.ToString(),
                    DeployedAt = firstDeploymentTime,
                    
                    PackageId = firstPackage.Id,
                    PackageDescription = firstPackage.Description,
                    
                    EnvironmentId = project.FirstEnvironment.Id,
                    EnvironmentDisplayName = project.FirstEnvironment.Name,
                    
                    ComponentId = project.FirstComponent.Id,
                    ComponentName = project.FirstComponent.Name
                },
                new GetDeploymentsListResponse.DeploymentDto
                {
                    Id = secondDeployment.CreatedDeploymentId,
                    Status = DeploymentStatus.Succeeded.ToString(),
                    DeployedAt = secondDeploymentTime,
                    
                    PackageId = secondPackage.Id,
                    PackageDescription = secondPackage.Description,

                    EnvironmentId = project.SecondEnvironment.Id,
                    EnvironmentDisplayName = project.SecondEnvironment.Name,
                    
                    ComponentId = project.FirstComponent.Id,
                    ComponentName = project.FirstComponent.Name
                }
            });
        }
    }
}