using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.Tests.TestProcess;
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
                    ZipFile = TestProcessZipArchives.TestProcess,
                    EnvironmentNames = new[] {"dev", "test", "prod"},
                    ComponentNames = new[] { "frontend", "backend" }
                };
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

            public Builder WithZipFile(IFormFile zipArchive)
            {
                _request.ZipFile = zipArchive;
                
                return this;
            }

            public async Task Create()
            {
                await _fixture.Handle(_request);
            }
        }
    }
}