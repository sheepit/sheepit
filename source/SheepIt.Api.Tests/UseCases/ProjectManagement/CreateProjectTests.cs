using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Model.Projects;
using SheepIt.Api.Tests.FeatureObjects;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectManagement;
using SheepIt.Api.UseCases.ProjectOperations.Components;
using SheepIt.Api.UseCases.ProjectOperations.Dashboard;
using SheepIt.Api.UseCases.ProjectOperations.Environments;

namespace SheepIt.Api.Tests.UseCases.ProjectManagement
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
                EnvironmentNames = new[] {"dev", "test", "prod"},
                ComponentNames = new[] { "frontend", "backend" }
            });

            // then

            var listProjectsResponse = await Fixture.Handle(new ListProjectsRequest());

            listProjectsResponse.Projects
                .Select(project => project.Id)
                .Should().Contain("foo");

            var getProjectDashboardResponse = await Fixture.Handle(new GetEnvironmentsListRequest
            {
                ProjectId = "foo"
            });

            getProjectDashboardResponse.Environments
                .Select(environment => environment.DisplayName)
                .Should().Equal("dev", "test", "prod");

            var listComponentsResponse = await Fixture.Handle(new ListComponentsRequest
            {
                ProjectId = "foo"
            });

            listComponentsResponse.Components
                .Select(component => component.Name)
                .Should().Equal("frontend", "backend");

            var getPackagesListResponse = await Fixture.Handle(new GetPackagesListRequest
            {
                ProjectId = "foo"
            });

            getPackagesListResponse.Packages
                .Select(package => package.ComponentName)
                .Should().Equal("frontend", "backend");
        }

        [Test]
        public async Task cannot_create_a_project_with_duplicated_id()
        {
            // given
            
            await Fixture.CreateProject()
                .WithId("foo")
                .Create();
            
            // when
            
            Func<Task> creatingProject = () => Fixture.CreateProject()
                .WithId("foo")
                .Create();

            creatingProject.Should().Throw<ProjectIdNotUniqueException>();
        }
    }
}