using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.Tests.TestProcess;
using SheepIt.Api.UseCases.ProjectManagement;
using SheepIt.Api.UseCases.ProjectOperations.Environments;

namespace SheepIt.Api.Tests.UseCases.ProjectOperations.Environments
{
    public class UpdateEnvironmentsTests : Test<IntegrationTestsFixture>
    {
        private string _projectId;

        [SetUp]
        public async Task set_up()
        {
            _projectId = "foo";

            var zipFile = TestProcessZip.GetAsFromFile();

            await Fixture.Handle(new CreateProjectRequest
            {
                ProjectId = _projectId,
                ZipFile = zipFile,
                EnvironmentNames = new[] {"test", "prod"}
            });
        }

        [Test]
        public async Task can_get_environments_for_editing()
        {
            // when

            var environmentsForUpdate = await Fixture.Handle(new GetEnvironmentsForUpdateRequest
            {
                ProjectId = _projectId
            });
            
            // then

            environmentsForUpdate.Environments.Should().BeEquivalentTo(new[]
            {
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = 1,
                    DisplayName = "test"
                },
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = 2,
                    DisplayName = "prod"
                }
            });
        }

        [Test]
        public async Task can_add_new_environment()
        {
            // when
            
            await Fixture.Handle(new UpdateEnvironmentsRequest
            {
                ProjectId = _projectId,
                Environments = new[]
                {
                    new UpdateEnvironmentsRequest.EnvironmentDto
                    {
                        Id = 0,
                        Rank = 0,
                        DisplayName = "dev" 
                    },
                    new UpdateEnvironmentsRequest.EnvironmentDto
                    {
                        Id = 1,
                        Rank = 1,
                        DisplayName = "test" 
                    },
                    new UpdateEnvironmentsRequest.EnvironmentDto
                    {
                        Id = 2,
                        Rank = 2,
                        DisplayName = "prod" 
                    }
                }
            });
            
            // then
            
            var environmentsForUpdate = await Fixture.Handle(new GetEnvironmentsForUpdateRequest
            {
                ProjectId = _projectId
            });
            
            environmentsForUpdate.Environments.Should().BeEquivalentTo(new[]
            {
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = 3,
                    DisplayName = "dev"
                },
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = 1,
                    DisplayName = "test"
                },
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = 2,
                    DisplayName = "prod"
                }
            });
        }

        [Test]
        public async Task can_change_environments_order()
        {
            // when
            
            await Fixture.Handle(new UpdateEnvironmentsRequest
            {
                ProjectId = _projectId,
                Environments = new[]
                {
                    new UpdateEnvironmentsRequest.EnvironmentDto
                    {
                        Id = 2,
                        Rank = 0,
                        DisplayName = "prod" 
                    },
                    new UpdateEnvironmentsRequest.EnvironmentDto
                    {
                        Id = 1,
                        Rank = 1,
                        DisplayName = "test" 
                    }
                }
            });
            
            // then
            
            var environmentsForUpdate = await Fixture.Handle(new GetEnvironmentsForUpdateRequest
            {
                ProjectId = _projectId
            });
            
            environmentsForUpdate.Environments.Should().BeEquivalentTo(new[]
            {
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = 2,
                    DisplayName = "prod"
                },
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = 1,
                    DisplayName = "test"
                }
            });
        }

        [Test]
        public async Task can_edit_existing_environments_details()
        {
            // when
            
            await Fixture.Handle(new UpdateEnvironmentsRequest
            {
                ProjectId = _projectId,
                Environments = new[]
                {
                    new UpdateEnvironmentsRequest.EnvironmentDto
                    {
                        Id = 1,
                        Rank = 0,
                        DisplayName = "modified test" 
                    },
                    new UpdateEnvironmentsRequest.EnvironmentDto
                    {
                        Id = 2,
                        Rank = 1,
                        DisplayName = "modified prod"
                    }
                }
            });
            
            // then
            
            var environmentsForUpdate = await Fixture.Handle(new GetEnvironmentsForUpdateRequest
            {
                ProjectId = _projectId
            });
            
            environmentsForUpdate.Environments.Should().BeEquivalentTo(new[]
            {
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = 1,
                    DisplayName = "modified test"
                },
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = 2,
                    DisplayName = "modified prod"
                }
            });
        }

        [Test]
        public async Task cannot_edit_nonexistent_environments_details()
        {
            // when
            
            Func<Task> updatingEnvironments = () => Fixture.Handle(new UpdateEnvironmentsRequest
            {
                ProjectId = _projectId,
                Environments = new[]
                {
                    new UpdateEnvironmentsRequest.EnvironmentDto
                    {
                        Id = 1,
                        Rank = 0,
                        DisplayName = "test" 
                    },
                    new UpdateEnvironmentsRequest.EnvironmentDto
                    {
                        Id = 2,
                        Rank = 1,
                        DisplayName = "prod"
                    },
                    new UpdateEnvironmentsRequest.EnvironmentDto
                    {
                        Id = 3,
                        Rank = 3,
                        DisplayName = "modified nonexistent environment"
                    }
                }
            });
            
            // then

            updatingEnvironments.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public async Task cannot_remove_environments()
        {
            // when
            
            await Fixture.Handle(new UpdateEnvironmentsRequest
            {
                ProjectId = _projectId,
                Environments = new[]
                {
                    new UpdateEnvironmentsRequest.EnvironmentDto
                    {
                        Id = 2,
                        Rank = 0,
                        DisplayName = "prod"
                    }
                }
            });
            
            // then
            
            // todo: this should probably fail, instead of doing nothing
            
            var environmentsForUpdate = await Fixture.Handle(new GetEnvironmentsForUpdateRequest
            {
                ProjectId = _projectId
            });

            environmentsForUpdate.Environments.Should().BeEquivalentTo(new[]
            {
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = 1,
                    DisplayName = "test"
                },
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = 2,
                    DisplayName = "prod"
                }
            });
        }
    }
}