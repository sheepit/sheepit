using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.UseCases;

namespace SheepIt.Api.Tests.UseCases
{
    public class CreateProjectTests : Test<IntegrationTestsFixture>
    {
        [Test]
        public async Task can_create_a_project()
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
            
            projects.Should().BeEquivalentTo(new ListProjectsResponse
            {
                Projects = new[]
                {
                    new ListProjectsResponse.ProjectDto
                    {
                        Id = "foo",
                        RepositoryUrl = "c:\\sheep-it\\sample-process"
                    }
                }
            });
        }
    }
}