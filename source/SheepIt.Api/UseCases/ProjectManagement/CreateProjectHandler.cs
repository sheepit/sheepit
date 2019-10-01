using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.DeploymentProcesses;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectManagement
{
    public class CreateProjectRequest : IRequest
    {
        public string ProjectId { get; set; }
        public string RepositoryUrl { get; set; }
        public string[] EnvironmentNames { get; set; }
        public IFormFile ZipFile { get; set; }
    }

    public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
    {
        public CreateProjectRequestValidator()
        {
            RuleFor(x => x.ProjectId)
                .NotNull()
                .MinimumLength(3);

            RuleFor(x => x.RepositoryUrl)
                .NotNull();

            RuleFor(x => x.EnvironmentNames)
                .NotNull();

            RuleForEach(x => x.EnvironmentNames)
                .NotNull()
                .MinimumLength(3);
            
            RuleFor(x => x.ZipFile)
                .NotNull();
        }
    }

    [Route("api")]
    [ApiController]
    public class CreateProjectController : MediatorController
    {
        [HttpPost]
        [Route("create-project")]
        [AllowAnonymous]
        public async Task CreateProject([FromForm] CreateProjectRequest model)
        {
            await Handle(model);
        }
    }

    public class CreateProjectHandler : IHandler<CreateProjectRequest>
    {
        private readonly Core.Environments.AddEnvironment _addEnvironment;
        private readonly ReleasesStorage _releasesStorage;
        private readonly SheepItDatabase _database;
        private readonly DeploymentProcessStorage _deploymentProcessStorage;

        public CreateProjectHandler(
            Core.Environments.AddEnvironment addEnvironment,
            ReleasesStorage releasesStorage,
            SheepItDatabase database,
            DeploymentProcessStorage deploymentProcessStorage)
        {
            _addEnvironment = addEnvironment;
            _releasesStorage = releasesStorage;
            _database = database;
            _deploymentProcessStorage = deploymentProcessStorage;
        }

        public async Task Handle(CreateProjectRequest request)
        {
            var project = new Project
            {
                Id = request.ProjectId,
                RepositoryUrl = request.RepositoryUrl,
            };

            await _database.Projects
                .InsertOneAsync(project);

            var deploymentProcessId = await CreateDeploymentProcess(request);

            await CreateEnvironments(request);

            // first release is created so other operations can copy it
            await CreateFirstRelease(project, deploymentProcessId);
        }

        private async Task CreateEnvironments(CreateProjectRequest request)
        {
            foreach (var environmentName in request.EnvironmentNames)
            {
                await _addEnvironment.Add(request.ProjectId, environmentName);
            }
        }
        
        private async Task CreateFirstRelease(Project project, int deploymentProcessId)
        {
            await _releasesStorage.Add(new Release
            {
                Variables = new VariableCollection(),
                CreatedAt = DateTime.UtcNow,
                ProjectId = project.Id,
                DeploymentProcessId = deploymentProcessId
            });
        }

        private async Task<int> CreateDeploymentProcess(
            CreateProjectRequest request)
        {
            using (var stream = new MemoryStream())
            {
                await request.ZipFile.CopyToAsync(stream);
                
                var deploymentProcessId = await _deploymentProcessStorage.Add(
                    request.ProjectId, stream.ToArray());

                return deploymentProcessId;
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