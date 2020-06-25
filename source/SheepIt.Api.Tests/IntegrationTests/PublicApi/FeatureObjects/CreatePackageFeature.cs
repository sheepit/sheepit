using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Tests.IntegrationTests.TestInfrastructure;
using SheepIt.Api.Tests.IntegrationTests.TestProcess;
using SheepIt.Api.UseCases.ProjectOperations.Packages;
using CreatePackageRequest = SheepIt.Api.PublicApi.Packages.CreatePackageRequest;

namespace SheepIt.Api.Tests.IntegrationTests.PublicApi.FeatureObjects
{
    public static class CreatePackageFeature
    {
        public static Builder CreatePackage(
            this IntegrationTestsFixture fixture, string projectId, int componentId)
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

            private readonly string _projectId;
            private readonly int _componentId;
            private readonly CreatePackageRequest _request;

            public Builder(IntegrationTestsFixture fixture, string projectId, int componentId)
            {
                _fixture = fixture;
                _projectId = projectId;
                _componentId = componentId;

                _request = new CreatePackageRequest
                {
                    Description = $"package {Guid.NewGuid()}",
                    ZipFile = TestProcessZipArchives.TestProcess,
                    VariableUpdates = null
                };
            }
            
            public Builder WithVariables(CreatePackageRequest.UpdateVariable[] variables)
            {
                _request.VariableUpdates = variables;
                return this;
            }

            public Builder WithoutDeploymentProcessZip()
            {
                _request.ZipFile = null;
                return this;
            }

            public async Task<CreatedPackage> Create()
            {
                if (_request.VariableUpdates == null)
                {
                    _request.VariableUpdates = await GetLastPackageVariables();
                }

                using var scope = _fixture.BeginDbContextScope();
            
                var createPackageHandler = scope.Resolve<SheepIt.Api.PublicApi.Packages.CreatePackageHandler>();
                
                var createPackageResponse = await createPackageHandler.Handle(_projectId, _componentId, _request);

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
                    ProjectId = _projectId,
                    ComponentId = _componentId
                });

                return getLastPackageResponse.Variables
                    .Select(variable => new CreatePackageRequest.UpdateVariable
                    {
                        Name = variable.Name,
                        DefaultValue = variable.DefaultValue,
                        EnvironmentValues = variable
                            .EnvironmentValues
                            .ToDictionary(pair => pair.Key.ToString(), pair => pair.Value)
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