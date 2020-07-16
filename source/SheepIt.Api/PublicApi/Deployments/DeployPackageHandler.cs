using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Model.Deployments;
using SheepIt.Api.Model.Packages;
using SheepIt.Api.UseCases.ProjectOperations.Deployments;

namespace SheepIt.Api.PublicApi.Deployments
{
    public class DeployPackageRequest : IRequest<DeployPackageResponse>
    {
        public int EnvironmentId { get; set; }
    }

    public class DeployPackageResponse
    {
        public int CreatedDeploymentId { get; set; }
    }

    [Route("api")]
    [ApiController]
    [AllowAnonymous]
    public class DeployPackageController
    {
        private readonly DeployPackageHandler _deployPackageHandler;

        public DeployPackageController(DeployPackageHandler deployPackageHandler)
        {
            _deployPackageHandler = deployPackageHandler;
        }
        
        [HttpPost]
        [Route("projects/{projectId}/components/{componentId}/packages/{packageId}/deployments")]
        public async Task<DeployPackageResponse> DeployPackage(string projectId, int componentId, int packageId, DeployPackageRequest request)
        {
            return await _deployPackageHandler.Handle(projectId, packageId, request);
        }
    }
    
    public class DeployPackageValidator : AbstractValidator<DeployPackageRequest>
    {
        public DeployPackageValidator()
        {
            RuleFor(request => request.EnvironmentId)
                .NotEqual(0);
        }
    }

    public class DeployPackageModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DeployPackageHandler>();
        }
    }

    public class DeployPackageHandler
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

        public async Task<DeployPackageResponse> Handle(string projectId, int packageId, DeployPackageRequest request)
        {
            var deployedPackage = await _dbContext.Packages
                .Include(package => package.Project)
                .Include(package => package.DeploymentProcess)
                .FindByIdAndProjectId(
                    packageId: packageId,
                    projectId: projectId
                );

            var deployment = await _deploymentFactory.Create(
                projectId: projectId,
                environmentId: request.EnvironmentId,
                packageId: packageId
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