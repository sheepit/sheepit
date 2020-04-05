using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Tests.FeatureObjects;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectOperations.Components;

namespace SheepIt.Api.Tests.UseCases.ProjectOperations.Components
{
    public class UpdateComponentsTests : Test<IntegrationTestsFixture>
    {
        private CreateProjectFeature.CreatedProject _project;

        private CreateProjectFeature.CreatedComponent Frontend => _project.FirstComponent;
        private CreateProjectFeature.CreatedComponent Backend => _project.SecondComponent;

        [SetUp]
        public async Task set_up()
        {
            _project = await Fixture.CreateProject()
                .WithComponents("frontend", "backend")
                .Create();
        }
        
        [Test]
        public async Task can_update_components()
        {
            // when
            
            await Fixture.Handle(new UpdateComponentsRequest
            {
                ProjectId = _project.Id,
                Components = new List<UpdateComponentsRequest.ComponentDto>
                {
                    new UpdateComponentsRequest.ComponentDto
                    {
                        Id = Backend.Id,
                        Name = "backend-service"
                    },
                    new UpdateComponentsRequest.ComponentDto
                    {
                        Id = null,
                        Name = "auth-service"
                    },
                    new UpdateComponentsRequest.ComponentDto
                    {
                        Id = Frontend.Id,
                        Name = Frontend.Name
                    }
                }
            });
            
            // then

            var listComponentsResponse = await Fixture.Handle(new ListComponentsRequest
            {
                ProjectId = _project.Id
            });

            listComponentsResponse.Components.Should().BeEquivalentTo(new[]
            {
                new ListComponentsResponse.ComponentDto
                {
                    Id = Backend.Id,
                    Name = "backend-service"
                },
                new ListComponentsResponse.ComponentDto
                {
                    Id = await Fixture.FindComponentId("auth-service"),
                    Name = "auth-service"
                },
                new ListComponentsResponse.ComponentDto
                {
                    Id = Frontend.Id,
                    Name = Frontend.Name
                }
            }, options => options.WithStrictOrdering());
        }

        [Test]
        public async Task cannot_remove_components()
        {
            // when
            
            Func<Task> removingComponent = () => Fixture.Handle(new UpdateComponentsRequest
            {
                ProjectId = _project.Id,
                Components = new List<UpdateComponentsRequest.ComponentDto>
                {
                    new UpdateComponentsRequest.ComponentDto
                    {
                        Id = Frontend.Id,
                        Name = Frontend.Name
                    }
                }
            });
            
            // then

            removingComponent.Should().Throw<InvalidOperationException>();
        }
    }
}