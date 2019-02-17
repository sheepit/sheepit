﻿using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure.Handlers;
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

    public class CreateProjectHandler : ISyncHandler<CreateProjectRequest>
    {
        private readonly ProjectsStorage _projectsStorage;
        private readonly Core.Environments.EnvironmentsStorage _environmentsStorage;
        private readonly ReleasesStorage _releasesStorage;
        private readonly DeploymentProcessGitRepositoryFactory _deploymentProcessGitRepositoryFactory;

        public CreateProjectHandler(ProjectsStorage projectsStorage, Core.Environments.EnvironmentsStorage environmentsStorage, ReleasesStorage releasesStorage, DeploymentProcessGitRepositoryFactory deploymentProcessGitRepositoryFactory)
        {
            _projectsStorage = projectsStorage;
            _environmentsStorage = environmentsStorage;
            _releasesStorage = releasesStorage;
            _deploymentProcessGitRepositoryFactory = deploymentProcessGitRepositoryFactory;
        }

        public void Handle(CreateProjectRequest request)
        {
            var project = new Project
            {
                Id = request.ProjectId,
                RepositoryUrl = request.RepositoryUrl
            };

            _projectsStorage.Add(project);

            CreateEnvironments(request);

            // first release is created so other operations can copy it
            CreateFirstRelease(project);
        }

        private void CreateEnvironments(CreateProjectRequest request)
        {
            foreach (var environmentName in request.EnvironmentNames)
            {
                var environment = new Environment(request.ProjectId, environmentName);
                
                _environmentsStorage.Add(environment);
            }
        }
        
        private void CreateFirstRelease(Project project)
        {
            var currentCommitSha = _deploymentProcessGitRepositoryFactory.GetCurrentCommitSha(project);

            _releasesStorage.Add(new Release
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
                .AsAsyncHandler()
                .RegisterIn(builder);
        }
    }
}