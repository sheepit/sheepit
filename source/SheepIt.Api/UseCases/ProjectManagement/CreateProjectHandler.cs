using System;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ProjectRepository _projectRepository;
        private readonly SheepItDbContext _dbContext;
        private readonly PackageFactory _packageFactory;
        private readonly DeploymentProcessFactory _deploymentProcessFactory;

        public CreateProjectHandler(
            Core.Environments.AddEnvironment addEnvironment,
            ValidateZipFile validateZipFile,
            ProjectRepository projectRepository,
            SheepItDbContext dbContext,
            PackageFactory packageFactory,
            DeploymentProcessFactory deploymentProcessFactory)
        {
            _addEnvironment = addEnvironment;
            _validateZipFile = validateZipFile;
            _projectRepository = projectRepository;
            _dbContext = dbContext;
            _packageFactory = packageFactory;
            _deploymentProcessFactory = deploymentProcessFactory;
        }

        public async Task Handle(CreateProjectRequest request)
        {
            // validation and persisting is split, as we don't have transactions yet
            // and failing midway would leave system in an inconsistent state
            
            // validation
            
            await ValidateProjectIdUniqueness(request.ProjectId);
            
            var zipFileBytes = await request.ZipFile.ToByteArray();
            
            _validateZipFile.Validate(zipFileBytes);

            // persisting

            _projectRepository.Create(new Project
            {
                Id = request.ProjectId
            });

            await _addEnvironment.AddMany(
                projectId: request.ProjectId,
                displayNames: request.EnvironmentNames
            );
            
            var deploymentProcess = await _deploymentProcessFactory.Create(
                projectId: request.ProjectId,
                zipFileBytes: zipFileBytes
            );

            _dbContext.DeploymentProcesses.Add(deploymentProcess);

            // first package is created so other operations can copy it
            var firstPackage = await _packageFactory.CreateFirstPackage(
                projectId: request.ProjectId,
                deploymentProcessId: deploymentProcess.Id
            );

            _dbContext.Packages.Add(firstPackage);

            await _dbContext.SaveChangesAsync();
        }

        private async Task ValidateProjectIdUniqueness(string projectId)
        {
            var duplicatedProject = await _projectRepository.TryGet(projectId);
            
            if (duplicatedProject != null)
            {
                throw new CustomException(
                    "CREATE_PROJECT_ID_NOT_UNIQUE",
                    "Project with specified id already exists"
                );
            }
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