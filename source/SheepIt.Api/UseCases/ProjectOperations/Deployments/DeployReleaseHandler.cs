﻿using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SheepIt.Api.Core.DeploymentProcesses;
using SheepIt.Api.Core.DeploymentProcessRunning;
using SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.Infrastructure.Time;

namespace SheepIt.Api.UseCases.ProjectOperations.Deployments
{
    public class DeployReleaseRequest : IRequest<DeployReleaseResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
        public int ReleaseId { get; set; }
        public int EnvironmentId { get; set; }
    }

    public class DeployReleaseResponse
    {
        public int CreatedDeploymentId { get; set; }
    }

    [Route("api")]
    [ApiController]
    public class DeployReleaseController : MediatorController
    {
        [HttpPost]
        [Route("project/deployment/deploy-release")]
        public async Task<DeployReleaseResponse> DeployRelease(DeployReleaseRequest request)
        {
            return await Handle(request);
        }
    }

    public class DeployReleaseHandler : IHandler<DeployReleaseRequest, DeployReleaseResponse>
    {
        private readonly DeploymentsStorage _deploymentsStorage;
        private readonly DeploymentProcessSettings _deploymentProcessSettings;
        private readonly DeploymentProcessGitRepositoryFactory _deploymentProcessGitRepositoryFactory;
        private readonly DeploymentProcessRunner _deploymentProcessRunner;
        private readonly IProjectContext _projectContext;
        private readonly SheepItDatabase _database;
        private readonly DeploymentProcessDirectoryFactory _deploymentProcessDirectoryFactory;
        private readonly DeploymentProcessStorage _deploymentProcessStorage;

        public DeployReleaseHandler(
            DeploymentsStorage deploymentsStorage,
            DeploymentProcessSettings deploymentProcessSettings,
            DeploymentProcessGitRepositoryFactory deploymentProcessGitRepositoryFactory,
            DeploymentProcessRunner deploymentProcessRunner,
            IProjectContext projectContext,
            SheepItDatabase database,
            DeploymentProcessDirectoryFactory deploymentProcessDirectoryFactory,
            DeploymentProcessStorage deploymentProcessStorage)
        {
            _deploymentsStorage = deploymentsStorage;
            _deploymentProcessSettings = deploymentProcessSettings;
            _deploymentProcessGitRepositoryFactory = deploymentProcessGitRepositoryFactory;
            _deploymentProcessRunner = deploymentProcessRunner;
            _projectContext = projectContext;
            _database = database;
            _deploymentProcessDirectoryFactory = deploymentProcessDirectoryFactory;
            _deploymentProcessStorage = deploymentProcessStorage;
        }

        public async Task<DeployReleaseResponse> Handle(DeployReleaseRequest request)
        {
            var release = await _database.Releases
                .FindByProjectAndId(request.ProjectId, request.ReleaseId);

            var deployment = new Deployment
            {
                ReleaseId = release.Id,
                ProjectId = request.ProjectId,
                DeployedAt = DateTime.UtcNow,
                EnvironmentId = request.EnvironmentId,
                Status = DeploymentStatus.InProgress
            };
            
            var deploymentId = await _deploymentsStorage.Add(deployment);

            var deploymentProcess = await _database.DeploymentProcesses
                .Find(builder => builder
                    .Eq(process => process.Id, release.DeploymentProcessId)
                )
                .SingleAsync();

            await RunDeployment(_projectContext.Project, release, deployment, deploymentProcess);

            return new DeployReleaseResponse
            {
                CreatedDeploymentId = deploymentId
            };
        }

        private async Task RunDeployment(Project project, Release release, Deployment deployment,
            DeploymentProcess deploymentProcess)
        {
            try
            {
                // todo: extract part of it to another class
                var deploymentWorkingDir = _deploymentProcessSettings.WorkingDir
                    .AddSegment(project.Id)
                    .AddSegment("deploying-releases")
                    .AddSegment($"{DateTime.UtcNow.FileFriendlyFormat()}_{deployment.EnvironmentId}_release-{release.Id}");

                // todo: make asynchronous
                var processDirectory = _deploymentProcessDirectoryFactory.CreateFromZip(
                    deploymentProcessZip: deploymentProcess.File,
                    toDirectory: deploymentWorkingDir
                );
                
                var processOutput = _deploymentProcessRunner.Run(
                    processDirectory.Path.ToString(),
                    deploymentProcessFile: processDirectory.OpenProcessDescriptionFile(),
                    variablesForEnvironment: release.GetVariablesForEnvironment(deployment.EnvironmentId)
                );

                deployment.MarkFinished(processOutput);

                await _database.Deployments
                    .ReplaceOneById(deployment);
            }
            catch (Exception)
            {
                // todo: log exception

                deployment.MarkExecutionFailed();

                await _database.Deployments
                    .ReplaceOneById(deployment);

                throw;
            }
        }
    }
    
    public class DeployReleaseModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<DeployReleaseHandler>()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }
}