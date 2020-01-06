using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.Core.DeploymentProcesses;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Core.Packages;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.ErrorHandling;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.Infrastructure.Web;

namespace SheepIt.Api.UseCases.ProjectManagement
{
    public class CreateProjectRequest : IRequest
    {
        public string ProjectId { get; set; }
        public string[] EnvironmentNames { get; set; }
        public IFormFile ZipFile { get; set; }
    }

    public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
    {
        public CreateProjectRequestValidator()
        {
            RuleFor(request => request.ProjectId)
                .NotNull()
                .MinimumLength(3);

            RuleFor(request => request.EnvironmentNames)
                .NotNull();

            RuleForEach(request => request.EnvironmentNames)
                .NotNull()
                 .MinimumLength(3);
            
            RuleFor(request => request.ZipFile)
                .NotNull();
        }
    }

    [Route("api")]
    [ApiController]
    public class CreateProjectController : MediatorController
    {
        [HttpPost]
        [Route("create-project")]
        public async Task CreateProject([FromForm] CreateProjectRequest model)
        {
            await Handle(model);
        }
    }

    public class CreateProjectHandler : IHandler<CreateProjectRequest>
    {
        private readonly SheepItDbContext _dbContext;
        private readonly PackageFactory _packageFactory;
        private readonly DeploymentProcessFactory _deploymentProcessFactory;
        private readonly EnvironmentFactory _environmentFactory;
        private readonly ProjectFactory _projectFactory;

        public CreateProjectHandler(
            SheepItDbContext dbContext,
            PackageFactory packageFactory,
            DeploymentProcessFactory deploymentProcessFactory,
            EnvironmentFactory environmentFactory,
            ProjectFactory projectFactory)
        {
            _dbContext = dbContext;
            _packageFactory = packageFactory;
            _deploymentProcessFactory = deploymentProcessFactory;
            _environmentFactory = environmentFactory;
            _projectFactory = projectFactory;
        }

        public async Task Handle(CreateProjectRequest request)
        {
            await CreateProject(
                projectId: request.ProjectId
            );

            await CreateEnvironments(
                projectId: request.ProjectId,
                environmentNames: request.EnvironmentNames
            );
            
            var deploymentProcess = await CreateDeploymentProcess(
                projectId: request.ProjectId,
                zipFile: request.ZipFile
            );

            // first package is created so other operations can copy it
            await CreatePackage(
                projectId: request.ProjectId,
                deploymentProcessId: deploymentProcess.Id
            );

            await _dbContext.SaveChangesAsync();
        }

        private async Task CreateProject(string projectId)
        {
            var project = await _projectFactory.Create(
                projectId: projectId
            );

            _dbContext.Projects.Add(project);
        }

        private async Task CreateEnvironments(string projectId, string[] environmentNames)
        {
            var currentEnvironmentRank = 1;
            
            foreach (var environmentName in environmentNames)
            {
                var environment = await _environmentFactory.Create(
                    projectId: projectId,
                    rank: currentEnvironmentRank,
                    displayName: environmentName
                );

                _dbContext.Environments.Add(environment);

                currentEnvironmentRank++;
            }
        }

        private async Task<DeploymentProcess> CreateDeploymentProcess(string projectId, IFormFile zipFile)
        {
            var deploymentProcess = await _deploymentProcessFactory.Create(
                projectId: projectId,
                zipFileBytes: await zipFile.ToByteArray()
            );

            _dbContext.DeploymentProcesses.Add(deploymentProcess);
            
            return deploymentProcess;
        }

        private async Task CreatePackage(string projectId, int deploymentProcessId)
        {
            var firstPackage = await _packageFactory.CreateFirstPackage(
                projectId: projectId,
                deploymentProcessId: deploymentProcessId
            );

            _dbContext.Packages.Add(firstPackage);
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