using System;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.Core.DeploymentProcessRunning;
using SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.Infrastructure.Time;
using SheepIt.Api.Model.Deployments;
using SheepIt.Api.Model.Packages;

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
    
    public class DeployPackageValidator : AbstractValidator<DeployPackageRequest>
    {
        public DeployPackageValidator()
        {
            RuleFor(request => request.ProjectId)
                .NotNull();

            RuleFor(request => request.PackageId)
                .NotEqual(0);
            
            RuleFor(request => request.EnvironmentId)
                .NotEqual(0);
        }
    }

    public class DeployPackageModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<DeployPackageHandler>()
                .InProjectLock()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class DeployPackageHandler : IHandler<DeployPackageRequest, DeployPackageResponse>
    {
        private readonly SheepItDbContext _dbContext;
        private readonly DeploymentFactory _deploymentFactory;
        private readonly RunDeployment _runDeployment;

        public DeployPackageHandler(
            SheepItDbContext dbContext,
            DeploymentFactory deploymentFactory,
            RunDeployment runDeployment)
        {
            _dbContext = dbContext;
            _deploymentFactory = deploymentFactory;
            _runDeployment = runDeployment;
        }

        public async Task<DeployPackageResponse> Handle(DeployPackageRequest request)
        {
            var deployedPackage = await _dbContext.Packages
                .Include(package => package.Project)
                .Include(package => package.DeploymentProcess)
                .FindByIdAndProjectId(
                    packageId: request.PackageId,
                    projectId: request.ProjectId
                );

            var deployment = await _deploymentFactory.Create(
                projectId: request.ProjectId,
                environmentId: request.EnvironmentId,
                packageId: request.PackageId
            );

            _dbContext.Deployments.Add(deployment);

            // todo: [rt] think if this double SaveChanges is a good solution
            await _dbContext.SaveChangesAsync();

            // todo: [rt] it looks like the deployment could run asynchronously
            _runDeployment.Run(
                project: deployedPackage.Project,
                package: deployedPackage,
                deployment: deployment,
                deploymentProcess: deployedPackage.DeploymentProcess
            );
            
            await _dbContext.SaveChangesAsync();

            return new DeployPackageResponse
            {
                CreatedDeploymentId = deployment.Id
            };
        }
    }
}