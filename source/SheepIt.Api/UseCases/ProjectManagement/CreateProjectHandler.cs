using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.Core.DeploymentProcesses;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Core.Packages;
using SheepIt.Api.DataAccess;
using SheepIt.Api.DataAccess.Sequencing;
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
        private readonly Core.Environments.AddEnvironment _addEnvironment;
        private readonly ValidateZipFile _validateZipFile;
        private readonly SheepItDbContext _dbContext;
        private readonly PackageFactory _packageFactory;
        private readonly DeploymentProcessFactory _deploymentProcessFactory;

        public CreateProjectHandler(
            Core.Environments.AddEnvironment addEnvironment,
            ValidateZipFile validateZipFile,
            SheepItDbContext dbContext,
            PackageFactory packageFactory,
            DeploymentProcessFactory deploymentProcessFactory)
        {
            _addEnvironment = addEnvironment;
            _validateZipFile = validateZipFile;
            _dbContext = dbContext;
            _packageFactory = packageFactory;
            _deploymentProcessFactory = deploymentProcessFactory;
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
            await ValidateProjectIdUniqueness(projectId);

            var project = new Project
            {
                Id = projectId
            };

            _dbContext.Projects.Add(project);
        }

        private async Task ValidateProjectIdUniqueness(string projectId)
        {
            var projectExists = await _dbContext.Projects
                .WithId(projectId)
                .AnyAsync();

            if (projectExists)
            {
                throw new CustomException(
                    "CREATE_PROJECT_ID_NOT_UNIQUE",
                    "Project with specified id already exists"
                );
            }
        }

        private async Task CreateEnvironments(string projectId, string[] environmentNames)
        {
            await _addEnvironment.AddMany(
                projectId: projectId,
                displayNames: environmentNames
            );
        }

        private async Task<DeploymentProcess> CreateDeploymentProcess(string projectId, IFormFile zipFile)
        {
            var zipFileBytes = await zipFile.ToByteArray();

            _validateZipFile.Validate(zipFileBytes);

            var deploymentProcess = await _deploymentProcessFactory.Create(
                projectId: projectId,
                zipFileBytes: zipFileBytes
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