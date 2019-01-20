using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Infrastructure;
using SheepIt.Api.Infrastructure.Api;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Domain;

namespace SheepIt.Api.UseCases
{
    public class CreateProjectRequest : IRequest
    {
        public string ProjectId { get; set; }
        public string RepositoryUrl { get; set; }
        public string[] EnvironmentNames { get; set; }
    }

    [Route("api")]
    [ApiController]
    public class CreateProjectController : MediatorController
    {
        [HttpPost]
        [Route("create-project")]
        public async Task CreateProject(CreateProjectRequest request)
        {
            await Handle(request);
        }
    }

    public class CreateProjectHandler : ISyncHandler<CreateProjectRequest>
    {
        private readonly Projects _projects;
        private readonly Domain.Environments _environments;
        private readonly ReleasesStorage _releasesStorage;

        public CreateProjectHandler(Projects projects, Domain.Environments environments, ReleasesStorage releasesStorage)
        {
            _projects = projects;
            _environments = environments;
            _releasesStorage = releasesStorage;
        }

        public void Handle(CreateProjectRequest request)
        {
            var project = new Project
            {
                Id = request.ProjectId,
                RepositoryUrl = request.RepositoryUrl
            };

            _projects.Add(project);

            CreateEnvironments(request);

            // first release is created so other operations can copy it
            CreateFirstRelease(project);
        }

        private void CreateEnvironments(CreateProjectRequest request)
        {
            foreach (var environmentName in request.EnvironmentNames)
            {
                var environment = new Domain.Environment(request.ProjectId, environmentName);
                
                _environments.Add(environment);
            }
        }
        
        private void CreateFirstRelease(Project project)
        {
            var currentCommitSha = ProcessRepository.GetCurrentCommitSha(project);

            _releasesStorage.Add(new Release
            {
                Variables = new VariableCollection(),
                CommitSha = currentCommitSha,
                CreatedAt = DateTime.UtcNow,
                ProjectId = project.Id
            });
        }
    }
}