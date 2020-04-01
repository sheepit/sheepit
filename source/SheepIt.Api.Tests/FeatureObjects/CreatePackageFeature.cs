using System;
using System.Linq;
using System.Threading.Tasks;
using SheepIt.Api.Infrastructure.Utils;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.Tests.TestProcess;
using SheepIt.Api.UseCases.ProjectOperations.Packages;

namespace SheepIt.Api.Tests.FeatureObjects
{
    public static class CreatePackageFeature
    {
        public static Builder CreatePackage(this IntegrationTestsFixture fixture, string projectId, int componentId)
        {
            return new Builder(
                fixture: fixture,
                projectId: projectId,
                componentId: componentId
            );
        }

        public class Builder
        {
            private readonly IntegrationTestsFixture _fixture;
            private readonly CreatePackageRequest _request;

            public Builder(IntegrationTestsFixture fixture, string projectId, int componentId)
            {
                _fixture = fixture;
                
                _request = new CreatePackageRequest
                {
                    ProjectId = projectId,
                    ComponentId = componentId,
                    Description = $"package {Guid.NewGuid()}",
                    ZipFile = TestProcessZipArchives.TestProcess,
                    VariableUpdates = null
                };
            }
            
            public Builder WithDescription(string description)
            {
                _request.Description = description;
                return this;
            }

            public Builder WithVariables(CreatePackageRequest.UpdateVariable[] variables)
            {
                _request.VariableUpdates = variables;
                return this;
            }

            public async Task<CreatedPackage> Create()
            {
                if (_request.VariableUpdates == null)
                {
                    _request.VariableUpdates = await GetLastPackageVariables();
                }

                var createPackageResponse = await _fixture.Handle(_request);

                return new CreatedPackage
                {
                    Id = createPackageResponse.CreatedPackageId,
                    Description = _request.Description
                };
            }

            private async Task<CreatePackageRequest.UpdateVariable[]> GetLastPackageVariables()
            {
                var getLastPackageResponse = await _fixture.Handle(new GetLastPackageRequest
                {
                    ProjectId = _request.ProjectId,
                    ComponentId = _request.ComponentId
                });

                return getLastPackageResponse.Variables
                    .Select(variable => new CreatePackageRequest.UpdateVariable
                    {
                        Name = variable.Name,
                        DefaultValue = variable.DefaultValue,
                        EnvironmentValues = variable.EnvironmentValues.ToDictionary()
                    })
                    .ToArray();
            }
        }

        public class CreatedPackage
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }
    }
}