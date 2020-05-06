using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.Packages;
using SheepIt.Api.Core.Packages.CreatePackage;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.ProjectContext;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectOperations.Packages
{
    public class CreatePackageRequest : IRequest<CreatePackageResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
        public int ComponentId { get; set; }
        
        public string Description { get; set; }
        public UpdateVariable[] VariableUpdates { get; set; }
        public IFormFile ZipFile { get; set; }

        public class UpdateVariable
        {
            public string Name { get; set; }
            public string DefaultValue { get; set; }
            public Dictionary<int, string> EnvironmentValues { get; set; }
        }
    }

    public class CreatePackageRequestValidator : AbstractValidator<CreatePackageRequest>
    {
        public CreatePackageRequestValidator()
        {
            RuleFor(request => request.ProjectId)
                .NotNull();
            
            RuleFor(request => request.Description)
                .NotNull();
            
            RuleFor(request => request.ZipFile)
                .NotNull();
        }
    }

    public class CreatePackageResponse
    {
        public int CreatedPackageId { get; set; }
    }

    [Route("frontendApi")]
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

    public class CreatePackageModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<CreatePackageHandler>()
                .InProjectLock()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class CreatePackageHandler : IHandler<CreatePackageRequest, CreatePackageResponse>
    {
        private readonly PackageService _packageService;

        public CreatePackageHandler(
            PackageService packageService)
        {
            _packageService = packageService;
        }

        public async Task<CreatePackageResponse> Handle(CreatePackageRequest request)
        {
            var command = new CreatePackageCommand
            {
                ProjectId = request.ProjectId,
                ComponentId = request.ComponentId,
                Description = request.Description,
                ZipFile = request.ZipFile,
                VariableUpdates = request.VariableUpdates
                    .Select(x => new
                        CreatePackageCommand.UpdateVariable {
                            Name = x.Name,
                            DefaultValue = x.DefaultValue,
                            EnvironmentValues = x.EnvironmentValues
                        })
                    .ToArray()
            };

            var createdPackageId = await _packageService.CreatePackage(command);

            return new CreatePackageResponse
            {
                CreatedPackageId = createdPackageId
            };
        }
    }
}