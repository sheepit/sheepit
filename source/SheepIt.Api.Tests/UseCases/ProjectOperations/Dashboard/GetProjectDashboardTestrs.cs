using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Tests.FeatureObjects;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectOperations.Dashboard;

namespace SheepIt.Api.Tests.UseCases.ProjectOperations.Dashboard
{
    public class GetProjectDashboardTests : Test<IntegrationTestsFixture>
    {
        [Test]
        public async Task can_get_projects_dashboard()
        {
            // given
            
            await Fixture.CreateProject("foo")
                .WithEnvironmentNames("dev", "test", "prod")
                .Create();
            
            // when

            var response = await Fixture.Handle(new GetProjectDashboardRequest
            {
                ProjectId = "foo"
            });
            
            // then

            response.Environments.Should().BeEquivalentTo(new[]
            {
                new GetProjectDashboardResponse.EnvironmentDto
                {
                    EnvironmentId = 1,
                    DisplayName = "dev",
                    Deployment = null
                },
                new GetProjectDashboardResponse.EnvironmentDto
                {
                    EnvironmentId = 2,
                    DisplayName = "test",
                    Deployment = null
                },
                new GetProjectDashboardResponse.EnvironmentDto
                {
                    EnvironmentId = 3,
                    DisplayName = "prod",
                    Deployment = null
                }
            });

            response.Deployments.Should().BeEmpty();

            response.Packages.Should().HaveCount(1);
        }
        
        // todo: [rt] test cases with additional packages and deployments
    }
}