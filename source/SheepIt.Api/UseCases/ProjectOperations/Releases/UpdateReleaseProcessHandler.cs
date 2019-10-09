using System.IO;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.DeploymentProcesses;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.UseCases.ProjectManagement;

namespace SheepIt.Api.UseCases.ProjectOperations.Releases
{
    public class UpdateReleaseProcessRequest : IRequest<UpdateReleaseProcessResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
        public IFormFile ZipFile { get; set; }
    }

    public class UpdateReleaseProcessRequestValidator : AbstractValidator<UpdateReleaseProcessRequest>
    {
        public UpdateReleaseProcessRequestValidator()
        {
            RuleFor(x => x.ProjectId)
                .NotNull();
            
            RuleFor(x => x.ZipFile)
                .NotNull();
        }
    }

    public class UpdateReleaseProcessResponse
    {
        public int CreatedReleaseId { get; set; }
    }

    [Route("api")]
    [ApiController]
    public class UpdateReleaseProcessController : MediatorController
    {
        [HttpPost]
        [Route("project/release/update-release-process")]
        public async Task<UpdateReleaseProcessResponse> UpdateReleaseProcess(
            [FromForm] UpdateReleaseProcessRequest request)
        {
            return await Handle(request);
        }
    }

    public class UpdateReleaseProcessHandler : IHandler<UpdateReleaseProcessRequest, UpdateReleaseProcessResponse>
    {
        private readonly ReleasesStorage _releasesStorage;
        private readonly IProjectContext _projectContext;
        private readonly DeploymentProcessStorage _deploymentProcessStorage;

        public UpdateReleaseProcessHandler(
            ReleasesStorage releasesStorage,
            IProjectContext projectContext,
            DeploymentProcessStorage deploymentProcessStorage)
        {
            _releasesStorage = releasesStorage;
            _projectContext = projectContext;
            _deploymentProcessStorage = deploymentProcessStorage;
        }

        public async Task<UpdateReleaseProcessResponse> Handle(UpdateReleaseProcessRequest request)
        {
            var project = _projectContext.Project;

            var deploymentProcessId = await _deploymentProcessStorage
                .Add(
                    project.Id,
                    request.ZipFile
                );

            var release = await _releasesStorage.GetNewest(request.ProjectId);

            var newRelease = release
                .WithUpdatedDeploymentProcess(deploymentProcessId);
            
            var releaseId = await _releasesStorage.Add(newRelease);

            return new UpdateReleaseProcessResponse
            {
                CreatedReleaseId = releaseId
            };
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
    
    public class UpdateReleaseProcessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<UpdateReleaseProcessHandler>()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }
}