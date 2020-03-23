using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Model.DeploymentProcesses;
using SheepIt.Api.Model.Projects;
using SheepIt.Api.Tests.FeatureObjects;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.Tests.TestProcess;
using SheepIt.Api.UseCases.ProjectManagement;
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
                ZipFile = TestProcessZipArchives.TestProcess
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
        }

        [Test]
        public async Task cannot_create_a_project_with_duplicated_id()
        {
            // given
            
            await Fixture.CreateProject("foo")
                .Create();
            
            // when
            
            Func<Task> creatingProject = () => Fixture.CreateProject("foo")
                .Create();

            creatingProject.Should().Throw<ProjectIdNotUniqueException>();
        }
        
        [Test]
        public void cannot_create_project_when_zip_file_does_not_contain_process_yaml()
        {
            // when

            Func<Task> creatingProjectWithSameName = () => Fixture
                .CreateProject("foo")
                .WithZipFile(TestProcessZipArchives.EmptyArchive)
                .Create();
            
            // then

            creatingProjectWithSameName.Should()
                .ThrowExactly<ZipArchiveDoesNotContainProcessYamlException>();
        }

        [Test]
        public void cannot_create_project_when_process_yaml_is_invalid()
        {
            // when
            
            Func<Task> creatingProjectWithSameName = () => Fixture
                .CreateProject("foo")
                .WithZipFile(TestProcessZipArchives.InvalidProcessYaml)
                .Create();
            
            // then

            creatingProjectWithSameName.Should()
                .ThrowExactly<ZipArchiveDeserializingFailedException>();
        }
    }
}