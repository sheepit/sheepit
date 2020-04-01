using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Model.Deployments;
using SheepIt.Api.Tests.FeatureObjects;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectOperations.DeploymentDetails;
using SheepIt.Api.UseCases.ProjectOperations.Deployments;

namespace SheepIt.Api.Tests.UseCases.ProjectOperations.Deployments
{
    public class DeployPackageTests : Test<IntegrationTestsFixture>
    {
        [Test]
        public async Task can_deploy_a_project()
        {
            // given

            var project = await Fixture.CreateProject("foo")
                .WithEnvironmentNames("dev", "test", "prod")
                .WithComponents("frontend", "backend")
                .Create();

            var createdPackage = await Fixture.CreatePackage(project.Id, project.FirstComponent.Id)
                .Create();

            // when

            var deployPackageResponse = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = project.Id,
                PackageId = createdPackage.Id,
                EnvironmentId = project.FirstEnvironment.Id
            });

            // then

            var getDeploymentDetailsResponse = await Fixture.Handle(new GetDeploymentDetailsRequest
            {
                ProjectId = project.Id,
                DeploymentId = deployPackageResponse.CreatedDeploymentId
            });

            getDeploymentDetailsResponse.Id.Should().Be(deployPackageResponse.CreatedDeploymentId);
            getDeploymentDetailsResponse.Status.Should().Be(DeploymentStatus.Succeeded.ToString());
        }
    }
}