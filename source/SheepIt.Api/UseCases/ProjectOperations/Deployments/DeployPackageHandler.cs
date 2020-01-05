using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SheepIt.Api.Core.DeploymentProcesses;
using SheepIt.Api.Core.DeploymentProcessRunning;
using SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Core.Packages;
using SheepIt.Api.DataAccess;
using SheepIt.Api.DataAccess.Sequencing;
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
        private readonly DeploymentProcessSettings _deploymentProcessSettings;
        private readonly DeploymentProcessRunner _deploymentProcessRunner;
        private readonly IProjectContext _projectContext;
        private readonly SheepItDatabase _database;
        private readonly DeploymentProcessDirectoryFactory _deploymentProcessDirectoryFactory;
        private readonly SheepItDbContext _dbContext;
        private readonly IClock _clock;
        private readonly IdStorage _idStorage;

        public DeployPackageHandler(
            DeploymentProcessSettings deploymentProcessSettings,
            DeploymentProcessRunner deploymentProcessRunner,
            IProjectContext projectContext,
            SheepItDatabase database,
            DeploymentProcessDirectoryFactory deploymentProcessDirectoryFactory,
            SheepItDbContext dbContext,
            IClock clock,
            IdStorage idStorage)
        {
            _deploymentProcessSettings = deploymentProcessSettings;
            _deploymentProcessRunner = deploymentProcessRunner;
            _projectContext = projectContext;
            _database = database;
            _deploymentProcessDirectoryFactory = deploymentProcessDirectoryFactory;
            _dbContext = dbContext;
            _clock = clock;
            _idStorage = idStorage;
        }

        public async Task<DeployPackageResponse> Handle(DeployPackageRequest request)
        {
            var package = await _dbContext.Packages.FindByIdAndProjectId(
                packageId: request.PackageId,
                projectId: request.ProjectId
            );

            var deployment = new Deployment
            {
                Id = await _idStorage.GetNext(IdSequence.Deployment),
                PackageId = package.Id,
                ProjectId = request.ProjectId,
                StartedAt = _clock.UtcNow,
                EnvironmentId = request.EnvironmentId,
                Status = DeploymentStatus.InProgress
            };

            _dbContext.Deployments.Add(deployment);

            var deploymentProcess = await _dbContext.DeploymentProcesses
                .Where(process => process.Id == package.DeploymentProcessId)
                .SingleAsync();

            // todo: [rt] think if this double SaveChanges is a good solution
            
            await _dbContext.SaveChangesAsync();

            // todo: [rt] it looks like the deployment could run asynchronously
            RunDeployment(_projectContext.Project, package, deployment, deploymentProcess);
            
            await _dbContext.SaveChangesAsync();

            return new DeployPackageResponse
            {
                CreatedDeploymentId = deployment.Id
            };
        }

        private void RunDeployment(Project project, Package package, Deployment deployment,
            DeploymentProcess deploymentProcess)
        {
            try
            {
                // todo: extract part of it to another class
                var deploymentWorkingDir = _deploymentProcessSettings.WorkingDir
                    .AddSegment(project.Id)
                    .AddSegment("deploying-packages")
                    .AddSegment($"{_clock.UtcNow.FileFriendlyFormat()}_{deployment.EnvironmentId}_package-{package.Id}");

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
            }
            catch (Exception)
            {
                // todo: log exception

                deployment.MarkExecutionFailed();

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