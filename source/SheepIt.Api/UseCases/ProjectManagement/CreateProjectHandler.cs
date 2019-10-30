using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.DeploymentProcesses;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Core.Packages;
using SheepIt.Api.Infrastructure.ErrorHandling;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
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
        private readonly PackagesStorage _packagesStorage;
        private readonly SheepItDatabase _database;
        private readonly DeploymentProcessStorage _deploymentProcessStorage;

        public CreateProjectHandler(
            Core.Environments.AddEnvironment addEnvironment,
            PackagesStorage packagesStorage,
            SheepItDatabase database,
            DeploymentProcessStorage deploymentProcessStorage)
        {
            _addEnvironment = addEnvironment;
            _packagesStorage = packagesStorage;
            _database = database;
            _deploymentProcessStorage = deploymentProcessStorage;
        }

        public async Task Handle(CreateProjectRequest request)
        {
            // validation and persisting is split, as we don't have transactions yet
            // and failing midway would leave system in an inconsistent state
            
            // validation
            
            await ValidateProjectIdUniqueness(request.ProjectId);
            
            var zipFileBytes = await request.ZipFile.ToByteArray();
            
            _deploymentProcessStorage.ValidateZipFile(zipFileBytes);

            // persisting
            
            await _database.Projects.InsertOneAsync(new Project
            {
                Id = request.ProjectId
            });

            foreach (var environmentName in request.EnvironmentNames)
            {
                await _addEnvironment.Add(request.ProjectId, environmentName);
            }

            var deploymentProcessId = await _deploymentProcessStorage.Add(
                projectId: request.ProjectId,
                zipFileBytes: zipFileBytes
            );

            // first package is created so other operations can copy it
            await _packagesStorage.Add(new Package
            {
                Variables = new VariableCollection(),
                CreatedAt = DateTime.UtcNow,
                ProjectId = request.ProjectId,
                DeploymentProcessId = deploymentProcessId
            });
        }

        private async Task ValidateProjectIdUniqueness(string projectId)
        {
            var duplicatedProject = await _database.Projects.TryFindById(projectId);
            
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