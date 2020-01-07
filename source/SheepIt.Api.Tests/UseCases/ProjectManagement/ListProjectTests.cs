using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Tests.FeatureObjects;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectManagement;

namespace SheepIt.Api.Tests.UseCases.ProjectManagement
{
    public class ListProjectTests : Test<IntegrationTestsFixture>
    {
        [Test]
        public async Task can_list_all_project()
        {
            // given

            await Fixture.CreateProject("aaa")
                .Create();
            
            await Fixture.CreateProject("bbb")
                .Create();
            
            await Fixture.CreateProject("ccc")
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
                        Id = "aaa"
                    },
                    new ListProjectsResponse.ProjectDto
                    {
                        Id = "bbb"
                    },
                    new ListProjectsResponse.ProjectDto
                    {
                        Id = "ccc"
                    }
                }
            });
        }
    }
}