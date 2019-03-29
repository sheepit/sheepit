using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectManagement;

namespace SheepIt.Api.Tests.FeatureObjects
{
    public static class CreateProjectFeature
    {
        public static Builder CreateProject(this IntegrationTestsFixture fixture, string projectId)
        {
            return new Builder(fixture, projectId);
        }

        public class Builder
        {
            private readonly IntegrationTestsFixture _fixture;
            private readonly CreateProjectRequest _request;

            public Builder(IntegrationTestsFixture fixture, string projectId)
            {
                _fixture = fixture;
                
                _request = new CreateProjectRequest
                {
                    ProjectId = projectId,
                    // todo: [ts] hardcoded paths will not work on other systems
                    RepositoryUrl = "c:\\sheep-it\\sample-process",
                    EnvironmentNames = new[] {"dev", "test", "prod"}
                };
            }

            public Builder WithEnvironmentNames(params string[] environmentNames)
            {
                _request.EnvironmentNames = environmentNames;

                return this;
            }

            public async Task Create()
            {
                await _fixture.Handle(_request);
            }
        }
    }
}