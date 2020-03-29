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
        // todo: this a quick fix, it should be inlined
        public static Builder CreatePackageForDefaultComponent(this IntegrationTestsFixture fixture, string projectId)
        {
            var latestPackageId = fixture.FindProjectsDefaultComponentId(projectId).Result;

            return new Builder(fixture, projectId, latestPackageId);
        }
        
        public static Builder CreatePackage(this IntegrationTestsFixture fixture, string projectId, int componentId)
        {
            return new Builder(fixture, projectId, componentId);
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
                    Description = Guid.NewGuid().ToString(),
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

            public async Task<CreatePackageResponse> Create()
            {
                if (_request.VariableUpdates == null)
                {
                    _request.VariableUpdates = await GetLastPackageVariables();
                }

                return await _fixture.Handle(_request);
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
    }
}