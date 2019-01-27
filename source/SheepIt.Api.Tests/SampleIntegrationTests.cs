using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.UseCases;

namespace SheepIt.Api.Tests
{
    public class SampleIntegrationTests : Test<IntegrationTestsFixture>
    {
        [Test]
        public async Task can_run_some_handler()
        {
            // given
            
            await Fixture.Handle(new CreateProjectRequest
            {
                ProjectId = "foo",
                RepositoryUrl = "c:\\sheep-it\\sample-process",
                EnvironmentNames = new[] {"dev", "test", "prod"}
            });
            
            // when

            var projects = await Fixture.Handle(new ListProjectsRequest());
            
            // then

            projects.Projects.Single().Should().BeEquivalentTo(new ListProjectsResponse.ProjectDto
            {
                Id = "foo",
                RepositoryUrl = "c:\\sheep-it\\sample-process"
            });
        }
    }
}