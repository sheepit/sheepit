using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.Infrastructure.Web;
using SheepIt.Api.Model.Components;
using SheepIt.Api.Model.DeploymentProcesses;
using SheepIt.Api.Model.Environments;
using SheepIt.Api.Model.Packages;
using SheepIt.Api.Model.Projects;

namespace SheepIt.Api.UseCases.ProjectManagement
{
    public class CreateProjectRequest : IRequest
    {
        public string ProjectId { get; set; }
        public string[] EnvironmentNames { get; set; }
        public string[] ComponentNames { get; set; }
    }

    public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
    {
        public CreateProjectRequestValidator()
        {
            RuleFor(request => request.ProjectId)
                .NotNull()
                .MinimumLength(3);

            RuleFor(request => request.EnvironmentNames)
                .ForEach(environment => environment.NotEmpty())
                .NotNull();
            
            RuleFor(request => request.ComponentNames)
                .ForEach(component => component.NotEmpty())
                .NotNull();

            RuleForEach(request => request.EnvironmentNames)
                .NotNull()
                 .MinimumLength(3);
        }
    }

    [Route("api")]
    [ApiController]
    public class CreateProjectController : MediatorController
    {
        [HttpPost]
        [Route("create-project")]
        public async Task CreateProject(CreateProjectRequest model)
        {
            await Handle(model);
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

    public class CreateProjectHandler : IHandler<CreateProjectRequest>
    {
        private readonly SheepItDbContext _dbContext;
        private readonly PackageFactory _packageFactory;
        private readonly DeploymentProcessFactory _deploymentProcessFactory;
        private readonly EnvironmentFactory _environmentFactory;
        private readonly ProjectFactory _projectFactory;
        private readonly ComponentFactory _componentFactory;

        public CreateProjectHandler(
            SheepItDbContext dbContext,
            PackageFactory packageFactory,
            DeploymentProcessFactory deploymentProcessFactory,
            EnvironmentFactory environmentFactory,
            ProjectFactory projectFactory,
            ComponentFactory componentFactory)
        {
            _dbContext = dbContext;
            _packageFactory = packageFactory;
            _deploymentProcessFactory = deploymentProcessFactory;
            _environmentFactory = environmentFactory;
            _projectFactory = projectFactory;
            _componentFactory = componentFactory;
        }

        public async Task Handle(CreateProjectRequest request)
        {
            var project = await _projectFactory.Create(
                projectId: request.ProjectId
            );
            
            _dbContext.Projects.Add(project);

            await CreateEnvironments(
                projectId: request.ProjectId,
                environmentNames: request.EnvironmentNames
            );

            var createdComponents = await CreateComponents(
                projectId: request.ProjectId,
                componentNames: request.ComponentNames
            );
            
            // first packages are created so other operations can copy them
            await CreateInitialPackages(
                projectId: request.ProjectId,
                createdComponents: createdComponents
            );

            await _dbContext.SaveChangesAsync();
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

                _dbContext.Add(environment);

                currentEnvironmentRank++;
            }
        }

        private async Task<List<CreatedComponent>> CreateComponents(string projectId, string[] componentNames)
        {
            var createdComponents = new List<CreatedComponent>();
            
            foreach (var componentName in componentNames)
            {
                var component = await _componentFactory.Create(
                    projectId: projectId,
                    name: componentName
                );
                
                _dbContext.Components.Add(component);
                
                createdComponents.Add(new CreatedComponent
                {
                    Id = component.Id,
                    Name = componentName
                });
            }

            return createdComponents;
        }

        private async Task CreateInitialPackages(string projectId, List<CreatedComponent> createdComponents)
        {
            // todo: move zip file or handle this path in a better way (e.g. with namespace, extract to another class etc.)
            var zipFileBytes = await File.ReadAllBytesAsync(@"UseCases\ProjectManagement\default-process.zip");

            foreach (var component in createdComponents)
            {
                var deploymentProcess = await _deploymentProcessFactory.Create(
                    projectId: projectId,
                    zipFileBytes: zipFileBytes
                );

                _dbContext.DeploymentProcesses.Add(deploymentProcess);

                var initialPackage = await _packageFactory.Create(
                    projectId: projectId,
                    deploymentProcessId: deploymentProcess.Id,
                    componentId: component.Id,
                    description: $"{component.Name} - initial package",
                    variableCollection: new VariableCollection()
                );
                
                _dbContext.Packages.Add(initialPackage);
            }
        }

        class CreatedComponent
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}