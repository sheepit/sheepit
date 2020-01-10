using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Tests.FeatureObjects;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.Tests.TestProcess;
using SheepIt.Api.UseCases.ProjectOperations.Packages;

namespace SheepIt.Api.Tests.UseCases.ProjectOperations.Packages
{
    public class GetLastPackageTests : Test<IntegrationTestsFixture>
    {
        [Test]
        public async Task can_get_last_package_from_project()
        {
            // given
            
            var projectId = "foo";

            await Fixture.CreateProject(projectId)
                .WithEnvironmentNames("test", "prod")
                .Create();

            await Fixture.CreatePackage("foo")
                .WithDescription("first")
                .Create();
            
            await Fixture.CreatePackage("foo")
                .WithDescription("second")
                .Create();
            
            await Fixture.CreatePackage("foo")
                .WithDescription("third")
                .WithVariables(new []
                {
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-1",
                        DefaultValue = "var-1-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {1, "var-1-test"},
                            {2, "var-1-prod"}
                        }
                    },
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-2",
                        DefaultValue = "var-2-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {1, "var-2-test"},
                            {2, "var-2-prod"}
                        }
                    }
                })
                .Create();
            
            // when

            var response = await Fixture.Handle(new GetLastPackageRequest
            {
                ProjectId = projectId
            });

            // then

            response.ProjectId.Should().Be(projectId);
            response.Description.Should().Be("third");

            response.Variables.Should().BeEquivalentTo(new[]
            {
                new GetLastPackageResponse.VariableDto
                {
                    Name = "var-1",
                    DefaultValue = "var-1-default",
                    EnvironmentValues = new Dictionary<int, string>
                    {
                        {1, "var-1-test"},
                        {2, "var-1-prod"}
                    }
                },
                new GetLastPackageResponse.VariableDto
                {
                    Name = "var-2",
                    DefaultValue = "var-2-default",
                    EnvironmentValues = new Dictionary<int, string>
                    {
                        {1, "var-2-test"},
                        {2, "var-2-prod"}
                    }
                }
            });
        }
    }
}