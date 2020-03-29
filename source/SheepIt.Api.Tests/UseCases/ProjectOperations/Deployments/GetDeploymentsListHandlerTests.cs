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
        private string _projectId;
        private int _devEnvironmentId;
        private int _testEnvironmentId;
        private int _frontendComponentId;

        [SetUp]
        public async Task set_up()
        {
            _projectId = "foo";

            await Fixture.CreateProject(_projectId)
                .WithEnvironmentNames("dev", "test", "prod")
                .WithComponents("frontend", "backend")
                .Create();

            _devEnvironmentId = await Fixture.FindEnvironmentId("dev");
            _testEnvironmentId = await Fixture.FindEnvironmentId("test");
            _frontendComponentId = await Fixture.FindComponentId("frontend");
            
            Fixture.MomentLater();
        }
        
        [Test]
        public async Task can_get_deployments()
        {
            // given

            var firstPackageDescription = "first package";
            
            var firstPackage = await Fixture.CreatePackageForDefaultComponent(_projectId)
                .WithDescription(firstPackageDescription)
                .Create();
            
            Fixture.MomentLater();

            var secondPackageDescription = "second package";
            
            var secondPackage = await Fixture.CreatePackageForDefaultComponent(_projectId)
                .WithDescription(secondPackageDescription)
                .Create();
            
            Fixture.MomentLater();

            var firstDeploymentTime = Fixture.GetUtcNow();

            var firstDeployment = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = _projectId,
                PackageId = firstPackage.CreatedPackageId,
                EnvironmentId = _devEnvironmentId
            });

            Fixture.MomentLater();

            var secondDeploymentTime = Fixture.GetUtcNow();

            var secondDeployment = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = _projectId,
                PackageId = secondPackage.CreatedPackageId,
                EnvironmentId = _testEnvironmentId
            });

            // when

            var response = await Fixture.Handle(new GetDeploymentsListRequest
            {
                ProjectId = _projectId
            });
            
            // then

            response.Deployments.Should().BeEquivalentTo(new[]
            {
                new GetDeploymentsListResponse.DeploymentDto
                {
                    Id = firstDeployment.CreatedDeploymentId,
                    Status = DeploymentStatus.Succeeded.ToString(),
                    DeployedAt = firstDeploymentTime,
                    EnvironmentId = _devEnvironmentId,
                    PackageId = firstPackage.CreatedPackageId,
                    PackageDescription = firstPackageDescription,
                    EnvironmentDisplayName = "dev",
                    ComponentId = _frontendComponentId,
                    ComponentName = "frontend"
                },
                new GetDeploymentsListResponse.DeploymentDto
                {
                    Id = secondDeployment.CreatedDeploymentId,
                    Status = DeploymentStatus.Succeeded.ToString(),
                    DeployedAt = secondDeploymentTime,
                    EnvironmentId = _testEnvironmentId,
                    PackageId = secondPackage.CreatedPackageId,
                    PackageDescription = secondPackageDescription,
                    EnvironmentDisplayName = "test",
                    ComponentId = _frontendComponentId,
                    ComponentName = "frontend"
                }
            });
        }
    }
}