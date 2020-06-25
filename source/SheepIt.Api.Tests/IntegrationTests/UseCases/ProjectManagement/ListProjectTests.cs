using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Tests.IntegrationTests.FeatureObjects;
using SheepIt.Api.Tests.IntegrationTests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectManagement;

namespace SheepIt.Api.Tests.IntegrationTests.UseCases.ProjectManagement
{
    public class ListProjectTests : Test<IntegrationTestsFixture>
    {
        [Test]
        public async Task can_list_all_project()
        {
            // given

            var firstProject = await Fixture.CreateProject()
                .Create();

            var secondProject = await Fixture.CreateProject()
                .Create();

            // when

            var response = await Fixture.Handle(new ListProjectsRequest());
            
            // then
            
            response.Should().BeEquivalentTo(new ListProjectsResponse
            {
                Projects = new []
                {
                    new ListProjectsResponse.ProjectDto
                    {
                        Id = firstProject.Id
                    },
                    new ListProjectsResponse.ProjectDto
                    {
                        Id = secondProject.Id
                    }
                }
            });
        }
    }
}