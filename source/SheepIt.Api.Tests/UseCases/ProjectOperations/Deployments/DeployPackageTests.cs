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
            
            await Fixture.CreateProject("foo")
                .WithEnvironmentNames("dev", "test", "prod")
                .Create();

            var devEnvironmentId = await Fixture.FindEnvironmentId("dev");
            var packageId = await Fixture.FindProjectsFirstPackageId("foo");

            // when

            var deployPackageResponse = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = "foo",
                PackageId = packageId,
                EnvironmentId = devEnvironmentId
            });

            // then

            var getDeploymentDetailsResponse = await Fixture.Handle(new GetDeploymentDetailsRequest
            {
                ProjectId = "foo",
                DeploymentId = deployPackageResponse.CreatedDeploymentId
            });

            getDeploymentDetailsResponse.Id.Should().Be(deployPackageResponse.CreatedDeploymentId);
            getDeploymentDetailsResponse.Status.Should().Be(DeploymentStatus.Succeeded.ToString());
        }
    }
}