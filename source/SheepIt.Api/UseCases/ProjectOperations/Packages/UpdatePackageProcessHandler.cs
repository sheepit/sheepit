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
    public class UpdatePackageProcessRequest : IRequest<UpdatePackageProcessResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
        public IFormFile ZipFile { get; set; }
    }

    public class UpdatePackageProcessRequestValidator : AbstractValidator<UpdatePackageProcessRequest>
    {
        public UpdatePackageProcessRequestValidator()
        {
            RuleFor(x => x.ProjectId)
                .NotNull();
            
            RuleFor(x => x.ZipFile)
                .NotNull();
        }
    }

    public class UpdatePackageProcessResponse
    {
        public int CreatedPackageId { get; set; }
    }

    [Route("api")]
    [ApiController]
    public class UpdatePackageProcessController : MediatorController
    {
        [HttpPost]
        [Route("project/package/update-package-process")]
        public async Task<UpdatePackageProcessResponse> UpdatePackageProcess(
            [FromForm] UpdatePackageProcessRequest request)
        {
            return await Handle(request);
        }
    }

    public class UpdatePackageProcessHandler : IHandler<UpdatePackageProcessRequest, UpdatePackageProcessResponse>
    {
        private readonly PackagesStorage _packagesStorage;
        private readonly IProjectContext _projectContext;
        private readonly DeploymentProcessStorage _deploymentProcessStorage;

        public UpdatePackageProcessHandler(
            PackagesStorage packagesStorage,
            IProjectContext projectContext,
            DeploymentProcessStorage deploymentProcessStorage)
        {
            _packagesStorage = packagesStorage;
            _projectContext = projectContext;
            _deploymentProcessStorage = deploymentProcessStorage;
        }

        public async Task<UpdatePackageProcessResponse> Handle(UpdatePackageProcessRequest request)
        {
            var zipFileBytes = await request.ZipFile.ToByteArray();
            
            _deploymentProcessStorage.ValidateZipFile(zipFileBytes);
            
            var deploymentProcessId = await _deploymentProcessStorage.Add(
                projectId: request.ProjectId,
                zipFileBytes: zipFileBytes
            );

            var package = await _packagesStorage.GetNewest(request.ProjectId);

            var newPackage = package.WithUpdatedDeploymentProcess(deploymentProcessId);
            
            var packageId = await _packagesStorage.Add(newPackage);

            return new UpdatePackageProcessResponse
            {
                CreatedPackageId = packageId
            };
        }
    }
    
    public class UpdatePackageProcessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<UpdatePackageProcessHandler>()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }
}