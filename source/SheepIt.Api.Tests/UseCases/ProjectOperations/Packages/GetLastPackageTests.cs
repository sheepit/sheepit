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
            var projectId = "foo";

            await Fixture.CreateProject(projectId)
                .WithEnvironmentNames("dev", "test", "prod")
                .Create();

            await Fixture.Handle(new CreatePackageRequest
            {
                ProjectId = projectId,
                Description = "first",
                ZipFile = TestProcessZipArchives.TestProcess,
                VariableUpdates = new[]
                {
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-1",
                        DefaultValue = "default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {1, "dev-value"},
                            {2, "test-value"},
                            {3, "prod-value"},
                        }
                    }
                }
            });

            var response = await Fixture.Handle(new GetLastPackageRequest
            {
                ProjectId = projectId
            });

            // then

            response.ProjectId.Should().Be(projectId);
            response.Variables.Should().BeEquivalentTo(new[]
            {
                new GetLastPackageResponse.VariableDto
                {
                    Name = "var-1",
                    DefaultValue = "default",
                    EnvironmentValues = new Dictionary<int, string>
                    {
                        {1, "dev-value"},
                        {2, "test-value"},
                        {3, "prod-value"},
                    }
                }
            });
        }
    }
}