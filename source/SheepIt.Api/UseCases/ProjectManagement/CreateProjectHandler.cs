using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;
using Environment = SheepIt.Api.Core.Environments.Environment;

namespace SheepIt.Api.UseCases.ProjectManagement
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

    public class CreateProjectHandler : IHandler<CreateProjectRequest>
    {
        private readonly Core.Environments.AddEnvironment _addEnvironment;
        private readonly ReleasesStorage _releasesStorage;
        private readonly DeploymentProcessGitRepositoryFactory _deploymentProcessGitRepositoryFactory;
        private readonly SheepItDatabase _database;

        public CreateProjectHandler(Core.Environments.AddEnvironment addEnvironment, ReleasesStorage releasesStorage, DeploymentProcessGitRepositoryFactory deploymentProcessGitRepositoryFactory, SheepItDatabase database)
        {
            _addEnvironment = addEnvironment;
            _releasesStorage = releasesStorage;
            _deploymentProcessGitRepositoryFactory = deploymentProcessGitRepositoryFactory;
            _database = database;
        }

        public async Task Handle(CreateProjectRequest request)
        {
            var project = new Project
            {
                Id = request.ProjectId,
                RepositoryUrl = request.RepositoryUrl
            };

            await _database.Projects
                .InsertOneAsync(project);

            await CreateEnvironments(request);

            // first release is created so other operations can copy it
            await CreateFirstRelease(project);
        }

        private async Task CreateEnvironments(CreateProjectRequest request)
        {
            foreach (var environmentName in request.EnvironmentNames)
            {
                await _addEnvironment.Add(request.ProjectId, environmentName);
            }
        }
        
        private async Task CreateFirstRelease(Project project)
        {
            var currentCommitSha = _deploymentProcessGitRepositoryFactory.GetCurrentCommitSha(project);

            await _releasesStorage.Add(new Release
            {
                Variables = new VariableCollection(),
                CommitSha = currentCommitSha,
                CreatedAt = DateTime.UtcNow,
                ProjectId = project.Id
            });
        }
    }

    public class CreateProjectModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<CreateProjectHandler>()
                .WithDefaultResponse()
                .RegisterAsHandlerIn(builder);
        }
    }
}