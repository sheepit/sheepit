using System.Linq;
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
                .ForEach(environment => environment.NotEmpty())
                .NotNull();
            
            RuleFor(request => request.ComponentNames)
                .ForEach(component => component.NotEmpty())
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

            var defaultComponentId = await CreateComponents(
                projectId: request.ProjectId,
                componentNames: request.ComponentNames
            );

            var deploymentProcess = await _deploymentProcessFactory.Create(
                projectId: request.ProjectId,
                zipFileBytes: await request.ZipFile.ToByteArray()
            );

            _dbContext.DeploymentProcesses.Add(deploymentProcess);

            // first package is created so other operations can copy it
            var firstPackage = await _packageFactory.Create(
                projectId: request.ProjectId,
                deploymentProcessId: deploymentProcess.Id,
                componentId: defaultComponentId,
                description: "Initial package",
                variableCollection: new VariableCollection()
            );

            _dbContext.Packages.Add(firstPackage);

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

        private async Task<int> CreateComponents(string projectId, string[] componentNames)
        {
            int? defaultComponentId = null;
            
            foreach (var componentName in componentNames)
            {
                var component = await _componentFactory.Create(
                    projectId: projectId,
                    name: componentName
                );
                
                _dbContext.Components.Add(component);

                if (componentName == componentNames.First())
                {
                    defaultComponentId = component.Id;
                }
            }

            return defaultComponentId.Value;
        }
    }
}