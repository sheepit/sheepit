using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Model.Deployments;
using SheepIt.Api.Tests.FeatureObjects;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectOperations.Deployments;
using SheepIt.Api.UseCases.ProjectOperations.Packages;

namespace SheepIt.Api.Tests.UseCases.ProjectOperations.Packages
{
    public class ListPackageDeploymentsTests : Test<IntegrationTestsFixture>
    {
        private string _projectId;
        private int _packageInQuestionId;
        private int _otherPackageId;

        [SetUp]
        public async Task set_up()
        {
            _projectId = "foo";
            
            await Fixture.CreateProject(_projectId)
                .WithEnvironmentNames("dev", "test", "prod")
                .Create();
            
            var packageInQuestion = await Fixture.CreatePackage(_projectId)
                .Create();

            _packageInQuestionId = packageInQuestion.CreatedPackageId;

            var otherPackage = await Fixture.CreatePackage(_projectId)
                .Create();

            _otherPackageId = otherPackage.CreatedPackageId;
        }

        [Test]
        public async Task list_contains_deployment_details()
        {
            // given

            var deploymentTime = Fixture.GetUtcNow();

            var deployPackageResponse = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = _projectId,
                PackageId = _packageInQuestionId,
                EnvironmentId = 1
            });
            
            // when

            var listPackageDeploymentsResponse = await Fixture.Handle(new ListPackageDeploymentsRequest
            {
                ProjectId = _projectId,
                PackageId = _packageInQuestionId
            });
            
            // then

            var deploymentDetails = listPackageDeploymentsResponse.Deployments.Single();
            
            deploymentDetails.Should().BeEquivalentTo(new ListPackageDeploymentsResponse.DeploymentDto
            {
                Id = deployPackageResponse.CreatedDeploymentId,
                Status = DeploymentStatus.Succeeded.ToString(),
                DeployedAt = deploymentTime,
                EnvironmentId = 1,
                EnvironmentDisplayName = "dev",
                PackageId = _packageInQuestionId
            });
        }
        
        [Test]
        public async Task deployments_from_other_packages_are_not_listed()
        {
            // given

            var deploymentInQuestion = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = _projectId,
                PackageId = _packageInQuestionId,
                EnvironmentId = 1
            });
            
            Fixture.MomentLater();

            await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = _projectId,
                PackageId = _otherPackageId,
                EnvironmentId = 1
            });
            
            // when

            var listPackageDeploymentsResponse = await Fixture.Handle(new ListPackageDeploymentsRequest
            {
                ProjectId = _projectId,
                PackageId = _packageInQuestionId
            });
            
            // then

            listPackageDeploymentsResponse.Deployments
                .Select(deployment => deployment.Id)
                .Should().Equal(deploymentInQuestion.CreatedDeploymentId);
        }
        
        [Test]
        public async Task deployments_are_listed_from_the_newest()
        {
            // given

            var firstDeployment = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = _projectId,
                PackageId = _packageInQuestionId,
                EnvironmentId = 1
            });
            
            Fixture.MomentLater();
            
            var secondDeployment = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = _projectId,
                PackageId = _packageInQuestionId,
                EnvironmentId = 2
            });
            
            // when

            var listPackageDeploymentsResponse = await Fixture.Handle(new ListPackageDeploymentsRequest
            {
                ProjectId = _projectId,
                PackageId = _packageInQuestionId
            });
            
            // then

            listPackageDeploymentsResponse.Deployments
                .Select(deployment => deployment.Id)
                .Should().Equal(
                    secondDeployment.CreatedDeploymentId,
                    firstDeployment.CreatedDeploymentId
                );
        }
    }
}