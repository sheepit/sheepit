using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.DeploymentProcesses;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Core.Packages;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.Infrastructure.Web;

namespace SheepIt.Api.UseCases.ProjectOperations.Packages
{
    public class CreatePackageRequest : IRequest<CreatePackageResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
        public string Description { get; set; }
        public UpdateVariable[] VariableUpdates { get; set; }
        public IFormFile ZipFile { get; set; }

        public class UpdateVariable
        {
            public string Name { get; set; }
            public string DefaultValue { get; set; }
            public Dictionary<int, string> EnvironmentValues { get; set; } =
                new Dictionary<int, string>();
        }
    }

    public class CreatePackageRequestValidator : AbstractValidator<CreatePackageRequest>
    {
        public CreatePackageRequestValidator()
        {
            RuleFor(x => x.ProjectId)
                .NotNull();
            
            RuleFor(x => x.Description)
                .NotNull();
            
            RuleFor(x => x.ZipFile)
                .NotNull();
        }
    }

    public class CreatePackageResponse
    {
        public int CreatedPackageId { get; set; }
    }

    [Route("api")]
    [ApiController]
    public class CreatePackageController : MediatorController
    {
        [HttpPost]
        [Route("project/package/create-package")]
        public async Task<CreatePackageResponse> CreatePackage(
            [FromForm] CreatePackageRequest request)
        {
            return await Handle(request);
        }
    }

    public class CreatePackageHandler : IHandler<CreatePackageRequest, CreatePackageResponse>
    {
        private readonly PackageRepository _packageRepository;
        private readonly DeploymentProcessStorage _deploymentProcessStorage;
        private readonly ValidateZipFile _validateZipFile;
        private readonly SheepItDbContext _dbContext;
        private readonly PackageFactory _packageFactory;

        public CreatePackageHandler(
            PackageRepository packageRepository,
            DeploymentProcessStorage deploymentProcessStorage,
            ValidateZipFile validateZipFile,
            SheepItDbContext dbContext,
            PackageFactory packageFactory)
        {
            _packageRepository = packageRepository;
            _deploymentProcessStorage = deploymentProcessStorage;
            _validateZipFile = validateZipFile;
            _dbContext = dbContext;
            _packageFactory = packageFactory;
        }

        public async Task<CreatePackageResponse> Handle(CreatePackageRequest request)
        {
            var package = await _packageRepository.GetNewest(request.ProjectId);

            var zipFileBytes = await request.ZipFile.ToByteArray();
            
            _validateZipFile.Validate(zipFileBytes);
            
            var deploymentProcessId = await _deploymentProcessStorage.Add(
                projectId: request.ProjectId,
                zipFileBytes: zipFileBytes
            );

            var variableValues = ComputeVariableValues(request);

            var newPackage = await _packageFactory.CreatePackageWithUpdatedProperties(
                package,
                variableValues,
                request.Description,
                deploymentProcessId
            );

            _dbContext.Packages.Add(newPackage);
            
            await _dbContext.SaveChangesAsync();

            return new CreatePackageResponse
            {
                CreatedPackageId = newPackage.Id
            };
        }

        private VariableValues[] ComputeVariableValues(CreatePackageRequest request)
        {
            return request
               .VariableUpdates
               ?.Select(update => VariableValues.Create(update.Name, update.DefaultValue, update.EnvironmentValues))
               .ToArray() ?? new VariableValues[0];
        }
    }
    
    public class CreatePackageModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<CreatePackageHandler>()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }
}