using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Tests.IntegrationTests.FeatureObjects;
using SheepIt.Api.Tests.IntegrationTests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectOperations.Packages;

namespace SheepIt.Api.Tests.IntegrationTests.UseCases.ProjectOperations.Packages
{
    public class GetLastPackageTests : Test<IntegrationTestsFixture>
    {
        [Test]
        public async Task can_get_last_package_from_project()
        {
            // given
            
            var project = await Fixture.CreateProject()
                .WithEnvironmentNames("test", "prod")
                .Create();
            
            var test = project.FirstEnvironment;
            var prod = project.SecondEnvironment;

            Fixture.MomentLater();

            await Fixture.CreatePackage(project.Id, project.FirstComponent.Id)
                .Create();
            
            Fixture.MomentLater();
            
            await Fixture.CreatePackage(project.Id, project.FirstComponent.Id)
                .Create();
            
            Fixture.MomentLater();

            var thirdPackage = await Fixture.CreatePackage(project.Id, project.FirstComponent.Id)
                .WithVariables(new []
                {
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-1",
                        DefaultValue = "var-1-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {test.Id, "var-1-test"},
                            {prod.Id, "var-1-prod"}
                        }
                    },
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-2",
                        DefaultValue = "var-2-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {test.Id, "var-2-test"},
                            {prod.Id, "var-2-prod"}
                        }
                    }
                })
                .Create();
            
            // when

            var response = await Fixture.Handle(new GetLastPackageRequest
            {
                ProjectId = project.Id,
                ComponentId = project.FirstComponent.Id
            });

            // then

            response.ProjectId.Should().Be(project.Id);
            response.Description.Should().Be(thirdPackage.Description);
            response.ComponentId.Should().Be(project.FirstComponent.Id);

            response.Variables.Should().BeEquivalentTo(new[]
            {
                new GetLastPackageResponse.VariableDto
                {
                    Name = "var-1",
                    DefaultValue = "var-1-default",
                    EnvironmentValues = new Dictionary<int, string>
                    {
                        {test.Id, "var-1-test"},
                        {prod.Id, "var-1-prod"}
                    }
                },
                new GetLastPackageResponse.VariableDto
                {
                    Name = "var-2",
                    DefaultValue = "var-2-default",
                    EnvironmentValues = new Dictionary<int, string>
                    {
                        {test.Id, "var-2-test"},
                        {prod.Id, "var-2-prod"}
                    }
                }
            });
        }
    }
}