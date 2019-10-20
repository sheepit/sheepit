using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Infrastructure.ErrorHandling;
using SheepIt.Api.Tests.FeatureObjects;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.Tests.TestProcess;
using SheepIt.Api.UseCases.ProjectOperations.Releases;

namespace SheepIt.Api.Tests.UseCases.Releases
{
    public class UpdateReleaseProcessTests : Test<IntegrationTestsFixture>
    {
        private const string ProjectId = "foo";

        [SetUp]
        public async Task set_up()
        {
            await Fixture
                .CreateProject(ProjectId)
                .Create();
        }

        [Test]
        public void can_update_process()
        {
            // when

            Func<Task> updatingProcess = () => Fixture.Handle(new UpdateReleaseProcessRequest
            {
                ProjectId = ProjectId,
                ZipFile = TestProcessZipArchives.TestProcess
            });
            
            // then
            
            updatingProcess.Should().NotThrow();
        }
        
        [Test]
        public void cannot_create_project_when_zip_file_does_not_contain_process_yaml()
        {
            // when
            
            Func<Task> updatingProcess = () => Fixture.Handle(new UpdateReleaseProcessRequest
            {
                ProjectId = ProjectId,
                ZipFile = TestProcessZipArchives.EmptyArchive
            });
            
            // then
            
            updatingProcess.Should().ThrowExactly<CustomException>()
                .Which.ErrorCode.Should().Be("CREATE_DEPLOYMENT_STORAGE_ZIP_DOES_NOT_CONTAIN_PROCESS_YAML");
        }

        [Test]
        public void cannot_create_project_when_process_yaml_is_invalid()
        {
            // when
            
            Func<Task> updatingProcess = () => Fixture.Handle(new UpdateReleaseProcessRequest
            {
                ProjectId = ProjectId,
                ZipFile = TestProcessZipArchives.InvalidProcessYaml
            });
            
            // then
            
            updatingProcess.Should().ThrowExactly<CustomException>()
                .Which.ErrorCode.Should().Be("CREATE_DEPLOYMENT_STORAGE_CANNOT_DESERIALIZE_PROCESS_YAML");
        }
    }
}