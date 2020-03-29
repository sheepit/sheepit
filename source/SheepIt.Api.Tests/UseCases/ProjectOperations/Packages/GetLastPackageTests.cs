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
                .WithComponents("frontend")
                .Create();
            
            var testEnvironmentId = await Fixture.FindEnvironmentId("test");
            var prodEnvironmentId = await Fixture.FindEnvironmentId("prod");
            var frontendComponentId = await Fixture.FindComponentId("frontend");

            Fixture.MomentLater();

            await Fixture.CreatePackageForDefaultComponent("foo")
                .WithDescription("first")
                .Create();
            
            Fixture.MomentLater();
            
            await Fixture.CreatePackageForDefaultComponent("foo")
                .WithDescription("second")
                .Create();
            
            Fixture.MomentLater();
            
            await Fixture.CreatePackageForDefaultComponent("foo")
                .WithDescription("third")
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
            
            // when

            var response = await Fixture.Handle(new GetLastPackageRequest
            {
                ProjectId = projectId,
                ComponentId = frontendComponentId
            });

            // then

            response.ProjectId.Should().Be(projectId);
            response.Description.Should().Be("third");
            response.ComponentId.Should().Be(frontendComponentId);

            response.Variables.Should().BeEquivalentTo(new[]
            {
                new GetLastPackageResponse.VariableDto
                {
                    Name = "var-1",
                    DefaultValue = "var-1-default",
                    EnvironmentValues = new Dictionary<int, string>
                    {
                        {testEnvironmentId, "var-1-test"},
                        {prodEnvironmentId, "var-1-prod"}
                    }
                },
                new GetLastPackageResponse.VariableDto
                {
                    Name = "var-2",
                    DefaultValue = "var-2-default",
                    EnvironmentValues = new Dictionary<int, string>
                    {
                        {testEnvironmentId, "var-2-test"},
                        {prodEnvironmentId, "var-2-prod"}
                    }
                }
            });
        }
    }
}