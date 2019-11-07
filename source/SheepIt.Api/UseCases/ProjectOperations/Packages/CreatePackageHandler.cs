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
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.Infrastructure.Web;

namespace SheepIt.Api.UseCases.ProjectOperations.Packages
{
    public class CreatePackageRequest : IRequest<CreatePackageResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
        public string Description { get; set; }
        public string[] VariableUpdatess { get; set; }
        public UpdateVariable[] VariableUpdates { get; set; }
        public IFormFile ZipFile { get; set; }

        public class UpdateVariable
        {
            public string Name { get; set; }
            public string DefaultValue { get; set; }
            public Dictionary<int, string> EnvironmentValues { get; set; }
        }
    }

//    public class CreatePackageRequestValidator : AbstractValidator<CreatePackageRequest>
//    {
//        public CreatePackageRequestValidator()
//        {
//            RuleFor(x => x.ProjectId)
//                .NotNull();
//            
//            RuleFor(x => x.ZipFile)
//                .NotNull();
//        }
//    }

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
        private readonly PackagesStorage _packagesStorage;
        private readonly DeploymentProcessStorage _deploymentProcessStorage;

        public CreatePackageHandler(
            PackagesStorage packagesStorage,
            DeploymentProcessStorage deploymentProcessStorage)
        {
            _packagesStorage = packagesStorage;
            _deploymentProcessStorage = deploymentProcessStorage;
        }

        public async Task<CreatePackageResponse> Handle(CreatePackageRequest request)
        {
            var package = await _packagesStorage.GetNewest(request.ProjectId);

            var variableValues = request
                .VariableUpdates?
                .Select(update => new VariableValues
                {
                    Name = update.Name,
                    DefaultValue = update.DefaultValue,
                    EnvironmentValues = update.EnvironmentValues
                })
                .ToArray() ?? new VariableValues[0];

            var zipFileBytes = await request.ZipFile.ToByteArray();
            
            _deploymentProcessStorage.ValidateZipFile(zipFileBytes);
            
            var deploymentProcessId = await _deploymentProcessStorage.Add(
                projectId: request.ProjectId,
                zipFileBytes: zipFileBytes
            );

            var newPackage = package.WithUpdatedVariables(variableValues);
            newPackage.Description = request.Description;
            newPackage.DeploymentProcessId = deploymentProcessId;

            var newPackageId = await _packagesStorage.Add(newPackage);

            return new CreatePackageResponse
            {
                CreatedPackageId = newPackageId
            };
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