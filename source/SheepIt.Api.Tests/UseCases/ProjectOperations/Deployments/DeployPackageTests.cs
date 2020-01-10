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
            
            // when

            await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = "foo",
                PackageId = 1,
                EnvironmentId = 1
            });
            
            // then

            var response = await Fixture.Handle(new GetDeploymentDetailsRequest
            {
                ProjectId = "foo",
                DeploymentId = 1
            });

            response.Id.Should().Be(1);
            response.Status.Should().Be(DeploymentStatus.Succeeded.ToString());
        }
    }
}