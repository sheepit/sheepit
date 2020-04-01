using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Tests.FeatureObjects;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectOperations.Environments;

namespace SheepIt.Api.Tests.UseCases.ProjectOperations.Environments
{
    public class ListEnvironmentsTests : Test<IntegrationTestsFixture>
    {
        [Test]
        public async Task can_list_project_environments()
        {
            // given

            var project = await Fixture.CreateProject("foo")
                .WithEnvironmentNames("test", "prod")
                .Create();

            // when

            var response = await Fixture.Handle(new ListEnvironmentsRequest
            {
                ProjectId = project.Id
            });

            // then

            response.Should().BeEquivalentTo(new ListEnvironmentsResponse
            {
                Environments = new[]
                {
                    new ListEnvironmentsResponse.EnvironmentDto
                    {
                        Id = project.FirstEnvironment.Id,
                        DisplayName = project.FirstEnvironment.Name,
                    },
                    new ListEnvironmentsResponse.EnvironmentDto
                    {
                        Id = project.SecondEnvironment.Id,
                        DisplayName = project.SecondEnvironment.Name
                    }
                }
            });
        }
    }
}