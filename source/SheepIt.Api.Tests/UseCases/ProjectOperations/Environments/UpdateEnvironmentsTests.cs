using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Tests.FeatureObjects;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.Tests.TestProcess;
using SheepIt.Api.UseCases.ProjectManagement;
using SheepIt.Api.UseCases.ProjectOperations.Environments;

namespace SheepIt.Api.Tests.UseCases.ProjectOperations.Environments
{
    public class UpdateEnvironmentsTests : Test<IntegrationTestsFixture>
    {
        private CreateProjectFeature.CreatedProject _project;
        
        private CreateProjectFeature.CreatedEnvironment _test;
        private CreateProjectFeature.CreatedEnvironment _prod;

        [SetUp]
        public async Task set_up()
        {
            _project = await Fixture.CreateProject()
                .WithEnvironmentNames("test", "prod")
                .Create();
            
            _test = _project.FirstEnvironment;
            _prod = _project.SecondEnvironment;
        }

        [Test]
        public async Task can_get_environments_for_editing()
        {
            // when

            var environmentsForUpdate = await Fixture.Handle(new GetEnvironmentsForUpdateRequest
            {
                ProjectId = _project.Id
            });
            
            // then

            environmentsForUpdate.Environments.Should().BeEquivalentTo(new[]
            {
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = _project.FirstEnvironment.Id,
                    DisplayName = _project.FirstEnvironment.Name
                },
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = _project.SecondEnvironment.Id,
                    DisplayName = _project.SecondEnvironment.Name
                }
            });
        }

        [Test]
        public async Task can_add_new_environment()
        {
            // when

            await Fixture.Handle(new UpdateEnvironmentsRequest
            {
                ProjectId = _project.Id,
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
                        Id = _test.Id,
                        Rank = 1,
                        DisplayName = _test.Name
                    },
                    new UpdateEnvironmentsRequest.EnvironmentDto
                    {
                        Id = _prod.Id,
                        Rank = 2,
                        DisplayName = _prod.Name
                    }
                }
            });
            
            // then
            
            var environmentsForUpdate = await Fixture.Handle(new GetEnvironmentsForUpdateRequest
            {
                ProjectId = _project.Id
            });

            var devId = await Fixture.FindEnvironmentId("dev");

            environmentsForUpdate.Environments.Should().BeEquivalentTo(new[]
            {
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = devId,
                    DisplayName = "dev"
                },
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = _test.Id,
                    DisplayName = _test.Name
                },
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = _prod.Id,
                    DisplayName = _prod.Name
                }
            });
        }

        [Test]
        public async Task can_change_environments_order()
        {
            // when
            
            await Fixture.Handle(new UpdateEnvironmentsRequest
            {
                ProjectId = _project.Id,
                Environments = new[]
                {
                    new UpdateEnvironmentsRequest.EnvironmentDto
                    {
                        Id = _prod.Id,
                        Rank = 0,
                        DisplayName = _prod.Name 
                    },
                    new UpdateEnvironmentsRequest.EnvironmentDto
                    {
                        Id = _test.Id,
                        Rank = 1,
                        DisplayName = _test.Name 
                    }
                }
            });
            
            // then
            
            var environmentsForUpdate = await Fixture.Handle(new GetEnvironmentsForUpdateRequest
            {
                ProjectId = _project.Id
            });
            
            environmentsForUpdate.Environments.Should().BeEquivalentTo(new[]
            {
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = _prod.Id,
                    DisplayName = _prod.Name
                },
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = _test.Id,
                    DisplayName = _test.Name
                }
            });
        }

        [Test]
        public async Task can_edit_existing_environments_details()
        {
            // when
            
            await Fixture.Handle(new UpdateEnvironmentsRequest
            {
                ProjectId = _project.Id,
                Environments = new[]
                {
                    new UpdateEnvironmentsRequest.EnvironmentDto
                    {
                        Id = _test.Id,
                        Rank = 0,
                        DisplayName = "modified test" 
                    },
                    new UpdateEnvironmentsRequest.EnvironmentDto
                    {
                        Id = _prod.Id,
                        Rank = 1,
                        DisplayName = "modified prod"
                    }
                }
            });
            
            // then
            
            var environmentsForUpdate = await Fixture.Handle(new GetEnvironmentsForUpdateRequest
            {
                ProjectId = _project.Id
            });
            
            environmentsForUpdate.Environments.Should().BeEquivalentTo(new[]
            {
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = _test.Id,
                    DisplayName = "modified test"
                },
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = _prod.Id,
                    DisplayName = "modified prod"
                }
            });
        }

        [Test]
        public void cannot_edit_nonexistent_environments_details()
        {
            // when
            
            Func<Task> updatingEnvironments = () => Fixture.Handle(new UpdateEnvironmentsRequest
            {
                ProjectId = _project.Id,
                Environments = new[]
                {
                    new UpdateEnvironmentsRequest.EnvironmentDto
                    {
                        Id = _test.Id,
                        Rank = 0,
                        DisplayName = _test.Name 
                    },
                    new UpdateEnvironmentsRequest.EnvironmentDto
                    {
                        Id = _prod.Id,
                        Rank = 1,
                        DisplayName = _prod.Name
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
                ProjectId = _project.Id,
                Environments = new[]
                {
                    new UpdateEnvironmentsRequest.EnvironmentDto
                    {
                        Id = _prod.Id,
                        Rank = 0,
                        DisplayName = _prod.Name
                    }
                }
            });
            
            // then
            
            // todo: this should probably fail, instead of doing nothing
            
            var environmentsForUpdate = await Fixture.Handle(new GetEnvironmentsForUpdateRequest
            {
                ProjectId = _project.Id
            });

            environmentsForUpdate.Environments.Should().BeEquivalentTo(new[]
            {
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = _test.Id,
                    DisplayName = _test.Name
                },
                new GetEnvironmentsForUpdateResponse.EnvironmentDto
                {
                    EnvironmentId = _prod.Id,
                    DisplayName = _prod.Name
                }
            });
        }
    }
}