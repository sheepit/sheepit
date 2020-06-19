using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Model.Components;
using SheepIt.Api.Model.Environments;
using SheepIt.Api.Tests.IntegrationTests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectManagement;

namespace SheepIt.Api.Tests.IntegrationTests.FeatureObjects
{
    public static class CreateProjectFeature
    {
        public static Builder CreateProject(this IntegrationTestsFixture fixture)
        {
            return new Builder(fixture);
        }

        public class Builder
        {
            private readonly IntegrationTestsFixture _fixture;
            private readonly CreateProjectRequest _request;

            public Builder(IntegrationTestsFixture fixture)
            {
                _fixture = fixture;
                
                _request = new CreateProjectRequest
                {
                    ProjectId = $"project-{Guid.NewGuid()}",
                    EnvironmentNames = new[] {"dev", "test", "prod"},
                    ComponentNames = new[] { "frontend", "backend", "other-service" }
                };
            }

            public Builder WithId(string id)
            {
                _request.ProjectId = id;

                return this;
            }

            public Builder WithEnvironmentNames(params string[] environmentNames)
            {
                _request.EnvironmentNames = environmentNames;

                return this;
            }

            public Builder WithComponents(params string[] components)
            {
                _request.ComponentNames = components;

                return this;
            }

            public async Task<CreatedProject> Create()
            {
                await _fixture.Handle(_request);

                using var scope = _fixture.BeginDbContextScope();
                
                var dbContext = scope.Resolve<SheepItDbContext>();

                return new CreatedProject
                {
                    Id = _request.ProjectId,

                    Environments = await dbContext
                        .Environments
                        .FromProject(_request.ProjectId)
                        .Select(environment => new CreatedEnvironment
                        {
                            Id = environment.Id,
                            Name = environment.DisplayName
                        })
                        .ToArrayAsync(),

                    Components = await dbContext
                        .Components
                        .FromProject(_request.ProjectId)
                        .Select(component => new CreatedComponent
                        {
                            Id = component.Id,
                            Name = component.Name
                        })
                        .ToArrayAsync()
                };
            }
        }

        public class CreatedProject
        {
            public string Id { get; set; }
            public CreatedEnvironment[] Environments { get; set; }
            public CreatedComponent[] Components { get; set; }

            public CreatedEnvironment FirstEnvironment => Environments[0];
            public CreatedEnvironment SecondEnvironment => Environments[1];
            public CreatedEnvironment ThirdEnvironment => Environments[2];
            
            public CreatedComponent FirstComponent => Components[0];
            public CreatedComponent SecondComponent => Components[1];
        }

        public class CreatedEnvironment
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        
        public class CreatedComponent
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}