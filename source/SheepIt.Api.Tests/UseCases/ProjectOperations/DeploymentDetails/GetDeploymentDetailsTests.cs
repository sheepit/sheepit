using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Model.Deployments;
using SheepIt.Api.Tests.FeatureObjects;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectOperations.DeploymentDetails;
using SheepIt.Api.UseCases.ProjectOperations.Deployments;
using SheepIt.Api.UseCases.ProjectOperations.Packages;

namespace SheepIt.Api.Tests.UseCases.ProjectOperations.DeploymentDetails
{
    public class GetDeploymentDetailsTests : Test<IntegrationTestsFixture>
    {
        [Test]
        public async Task can_get_deployment_details()
        {
            // given

            var projectId = "foo";

            await Fixture.CreateProject(projectId)
                .WithEnvironmentNames("test", "prod")
                .Create();
            
            var testEnvironmentId = await Fixture.FindEnvironmentId("test");
            var prodEnvironmentId = await Fixture.FindEnvironmentId("prod");

            var description = "some package";
            
            var createPackageResponse = await Fixture.CreatePackage(projectId)
                .WithDescription(description)
                .WithVariables(new []
                {
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-1",
                        DefaultValue = "var-1-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {testEnvironmentId, "var-1-test"},
                            {prodEnvironmentId, "var-1-prod"}
                        }
                    },
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-2",
                        DefaultValue = "var-2-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {testEnvironmentId, "var-2-test"},
                            {prodEnvironmentId, "var-2-prod"}
                        }
                    }
                })
                .Create();

            var deployPackageResponse = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = projectId,
                PackageId = createPackageResponse.CreatedPackageId,
                EnvironmentId = testEnvironmentId
            });

            // when

            var getDeploymentDetailsResponse = await Fixture.Handle(new GetDeploymentDetailsRequest
            {
                ProjectId = projectId,
                DeploymentId = deployPackageResponse.CreatedDeploymentId
            });
            
            // then
            
            getDeploymentDetailsResponse.Should().BeEquivalentTo(new GetDeploymentDetailsResponse
            {
                Id = deployPackageResponse.CreatedDeploymentId,
                Status = DeploymentStatus.Succeeded.ToString(),
                DeployedAt = Fixture.GetUtcNow(),
                EnvironmentId = testEnvironmentId,
                PackageDescription = description,
                PackageId = createPackageResponse.CreatedPackageId,
                EnvironmentDisplayName = "test",
                Variables = new GetDeploymentDetailsResponse.VariablesForEnvironmentDto[]
                {
                    new GetDeploymentDetailsResponse.VariablesForEnvironmentDto
                    {
                        Name = "var-1",
                        Value = "var-1-test"
                    },
                    new GetDeploymentDetailsResponse.VariablesForEnvironmentDto
                    {
                        Name = "var-2",
                        Value = "var-2-test"
                    }
                }
            }, options => options.Excluding(response => response.StepResults));
        }
    }
}