using System;
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
using SheepIt.Api.Core.Packages;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.Infrastructure.Time;

namespace SheepIt.Api.UseCases.ProjectOperations.Deployments
{
    public class DeployPackageRequest : IRequest<DeployPackageResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
        public int PackageId { get; set; }
        public int EnvironmentId { get; set; }
    }

    public class DeployPackageResponse
    {
        public int CreatedDeploymentId { get; set; }
    }

    [Route("api")]
    [ApiController]
    public class DeployPackageController : MediatorController
    {
        [HttpPost]
        [Route("project/deployment/deploy-package")]
        public async Task<DeployPackageResponse> DeployPackage(DeployPackageRequest request)
        {
            return await Handle(request);
        }
    }

    public class DeployPackageHandler : IHandler<DeployPackageRequest, DeployPackageResponse>
    {
        private readonly DeploymentsStorage _deploymentsStorage;
        private readonly DeploymentProcessSettings _deploymentProcessSettings;
        private readonly DeploymentProcessRunner _deploymentProcessRunner;
        private readonly IProjectContext _projectContext;
        private readonly SheepItDatabase _database;
        private readonly DeploymentProcessDirectoryFactory _deploymentProcessDirectoryFactory;
        private readonly DeploymentProcessStorage _deploymentProcessStorage;

        public DeployPackageHandler(
            DeploymentsStorage deploymentsStorage,
            DeploymentProcessSettings deploymentProcessSettings,
            DeploymentProcessRunner deploymentProcessRunner,
            IProjectContext projectContext,
            SheepItDatabase database,
            DeploymentProcessDirectoryFactory deploymentProcessDirectoryFactory,
            DeploymentProcessStorage deploymentProcessStorage)
        {
            _deploymentsStorage = deploymentsStorage;
            _deploymentProcessSettings = deploymentProcessSettings;
            _deploymentProcessRunner = deploymentProcessRunner;
            _projectContext = projectContext;
            _database = database;
            _deploymentProcessDirectoryFactory = deploymentProcessDirectoryFactory;
            _deploymentProcessStorage = deploymentProcessStorage;
        }

        public async Task<DeployPackageResponse> Handle(DeployPackageRequest request)
        {
            var package = await _database.Packages
                .FindByProjectAndId(request.ProjectId, request.PackageId);

            var deployment = new Deployment
            {
                PackageId = package.Id,
                ProjectId = request.ProjectId,
                DeployedAt = DateTime.UtcNow,
                EnvironmentId = request.EnvironmentId,
                Status = DeploymentStatus.InProgress
            };
            
            var deploymentId = await _deploymentsStorage.Add(deployment);

            var deploymentProcess = await _database.DeploymentProcesses
                .Find(builder => builder
                    .Eq(process => process.Id, package.DeploymentProcessId)
                )
                .SingleAsync();

            await RunDeployment(_projectContext.Project, package, deployment, deploymentProcess);

            return new DeployPackageResponse
            {
                CreatedDeploymentId = deploymentId
            };
        }

        private async Task RunDeployment(Project project, PackageMongoEntity package, Deployment deployment,
            DeploymentProcess deploymentProcess)
        {
            try
            {
                // todo: extract part of it to another class
                var deploymentWorkingDir = _deploymentProcessSettings.WorkingDir
                    .AddSegment(project.Id)
                    .AddSegment("deploying-packages")
                    .AddSegment($"{DateTime.UtcNow.FileFriendlyFormat()}_{deployment.EnvironmentId}_package-{package.Id}");

                // todo: make asynchronous
                var processDirectory = _deploymentProcessDirectoryFactory.CreateFromZip(
                    deploymentProcessZip: deploymentProcess.File,
                    toDirectory: deploymentWorkingDir
                );
                
                var processOutput = _deploymentProcessRunner.Run(
                    processDirectory.Path.ToString(),
                    deploymentProcessFile: processDirectory.OpenProcessDescriptionFile(),
                    variablesForEnvironment: package.GetVariablesForEnvironment(deployment.EnvironmentId)
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
    
    public class DeployPackageModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<DeployPackageHandler>()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }
}