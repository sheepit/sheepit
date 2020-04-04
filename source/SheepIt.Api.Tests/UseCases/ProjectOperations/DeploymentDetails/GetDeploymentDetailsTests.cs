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

            var project = await Fixture.CreateProject()
                .WithEnvironmentNames("test", "prod")
                .Create();

            var createdPackage = await Fixture
                .CreatePackage(project.Id, project.FirstComponent.Id)
                .WithVariables(new []
                {
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-1",
                        DefaultValue = "var-1-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {project.FirstEnvironment.Id, "var-1-test"},
                            {project.SecondEnvironment.Id, "var-1-prod"}
                        }
                    },
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-2",
                        DefaultValue = "var-2-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {project.FirstEnvironment.Id, "var-2-test"},
                            {project.SecondEnvironment.Id, "var-2-prod"}
                        }
                    }
                })
                .Create();

            var deployPackageResponse = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = project.Id,
                PackageId = createdPackage.Id,
                EnvironmentId = project.FirstEnvironment.Id
            });

            // when

            var getDeploymentDetailsResponse = await Fixture.Handle(new GetDeploymentDetailsRequest
            {
                ProjectId = project.Id,
                DeploymentId = deployPackageResponse.CreatedDeploymentId
            });
            
            // then
            
            getDeploymentDetailsResponse.Should().BeEquivalentTo(new GetDeploymentDetailsResponse
            {
                Id = deployPackageResponse.CreatedDeploymentId,
                Status = DeploymentStatus.Succeeded.ToString(),
                DeployedAt = Fixture.GetUtcNow(),
                
                EnvironmentId = project.FirstEnvironment.Id,
                EnvironmentDisplayName = project.FirstEnvironment.Name,
                
                ComponentId = project.FirstComponent.Id,
                ComponentName = project.FirstComponent.Name,
                
                PackageId = createdPackage.Id,
                PackageDescription = createdPackage.Description,
                
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