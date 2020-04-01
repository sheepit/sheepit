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
        private CreateProjectFeature.CreatedProject _project;

        private CreateProjectFeature.CreatedEnvironment _dev => _project.FirstEnvironment;
        private CreateProjectFeature.CreatedEnvironment _test => _project.SecondEnvironment;

        private CreatePackageFeature.CreatedPackage _packageInQuestion;
        private CreatePackageFeature.CreatedPackage _otherPackage;

        [SetUp]
        public async Task set_up()
        {
            _project = await Fixture.CreateProject("foo")
                .WithEnvironmentNames("dev", "test", "prod")
                .Create();

            _packageInQuestion = await Fixture.CreatePackage(_project.Id, _project.FirstComponent.Id)
                .Create();

            _otherPackage = await Fixture.CreatePackage(_project.Id, _project.FirstComponent.Id)
                .Create();
        }

        [Test]
        public async Task list_contains_deployment_details()
        {
            // given

            var deploymentTime = Fixture.GetUtcNow();

            var deployPackageResponse = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = _project.Id,
                PackageId = _packageInQuestion.Id,
                EnvironmentId = _dev.Id
            });
            
            // when

            var listPackageDeploymentsResponse = await Fixture.Handle(new ListPackageDeploymentsRequest
            {
                ProjectId = _project.Id,
                PackageId = _packageInQuestion.Id
            });
            
            // then

            var deploymentDetails = listPackageDeploymentsResponse.Deployments.Single();
            
            deploymentDetails.Should().BeEquivalentTo(new ListPackageDeploymentsResponse.DeploymentDto
            {
                Id = deployPackageResponse.CreatedDeploymentId,
                Status = DeploymentStatus.Succeeded.ToString(),
                DeployedAt = deploymentTime,
                EnvironmentId = _dev.Id,
                EnvironmentDisplayName = _dev.Name,
                PackageId = _packageInQuestion.Id
            });
        }
        
        [Test]
        public async Task deployments_from_other_packages_are_not_listed()
        {
            // given

            var deploymentInQuestion = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = _project.Id,
                PackageId = _packageInQuestion.Id,
                EnvironmentId = _dev.Id
            });
            
            Fixture.MomentLater();

            await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = _project.Id,
                PackageId = _otherPackage.Id,
                EnvironmentId = _dev.Id
            });
            
            // when

            var listPackageDeploymentsResponse = await Fixture.Handle(new ListPackageDeploymentsRequest
            {
                ProjectId = _project.Id,
                PackageId = _packageInQuestion.Id
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
                ProjectId = _project.Id,
                PackageId = _packageInQuestion.Id,
                EnvironmentId = _dev.Id
            });
            
            Fixture.MomentLater();
            
            var secondDeployment = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = _project.Id,
                PackageId = _packageInQuestion.Id,
                EnvironmentId = _test.Id
            });
            
            // when

            var listPackageDeploymentsResponse = await Fixture.Handle(new ListPackageDeploymentsRequest
            {
                ProjectId = _project.Id,
                PackageId = _packageInQuestion.Id
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