using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http.Internal;
using NUnit.Framework;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.Tests.TestProcess;
using SheepIt.Api.UseCases.ProjectManagement;

namespace SheepIt.Api.Tests.UseCases
{
    public class CreateProjectTests : Test<IntegrationTestsFixture>
    {
        [Test]
        public async Task can_create_a_project()
        {
            // when

            await Fixture.Handle(new CreateProjectRequest
            {
                ProjectId = "foo",
                ZipFile = TestProcessZip.GetAsFromFile(),
                EnvironmentNames = new[] {"dev", "test", "prod"}
            });
            
            // then
            
            var projects = await Fixture.Handle(new ListProjectsRequest());
            
            projects.Should().BeEquivalentTo(new ListProjectsResponse
            {
                Projects = new[]
                {
                    new ListProjectsResponse.ProjectDto
                    {
                        Id = "foo",
                        RepositoryUrl = null
                    }
                }
            });
        }
    }
}