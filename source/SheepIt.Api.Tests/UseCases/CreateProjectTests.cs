using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Infrastructure.ErrorHandling;
using SheepIt.Api.Tests.FeatureObjects;
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
                ZipFile = TestProcessZipArchives.TestProcess,
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
                        Id = "foo"
                    }
                }
            });
        }

        [Test]
        public async Task cannot_create_project_with_duplicated_id()
        {
            // given

            await Fixture.CreateProject("foo")
                .Create();
            
            // when
            
            Func<Task> creatingProjectWithSameName = () => Fixture
                .CreateProject("foo")
                .Create();
            
            // then

            creatingProjectWithSameName.Should().ThrowExactly<CustomException>()
                .Which.ErrorCode.Should().Be("CREATE_PROJECT_ID_NOT_UNIQUE");
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
            
            creatingProjectWithSameName.Should().ThrowExactly<CustomException>()
                .Which.ErrorCode.Should().Be("CREATE_DEPLOYMENT_STORAGE_ZIP_DOES_NOT_CONTAIN_PROCESS_YAML");
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
            
            creatingProjectWithSameName.Should().ThrowExactly<CustomException>()
                .Which.ErrorCode.Should().Be("CREATE_DEPLOYMENT_STORAGE_CANNOT_DESERIALIZE_PROCESS_YAML");
        }
    }
}