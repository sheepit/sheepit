using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.DeploymentProcesses;
using SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectOperations.Releases
{
    public class UpdateReleaseProcessRequest : IRequest<UpdateReleaseProcessResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
    }

    public class UpdateReleaseProcessResponse
    {
        public int CreatedReleaseId { get; set; }
        public string CreatedFromCommitSha { get; set; }
    }

    [Route("api")]
    [ApiController]
    public class UpdateReleaseProcessController : MediatorController
    {
        [HttpPost]
        [Route("project/release/update-release-process")]
        public async Task<UpdateReleaseProcessResponse> UpdateReleaseProcess(UpdateReleaseProcessRequest request)
        {
            return await Handle(request);
        }
    }

    public class UpdateReleaseProcessHandler : IHandler<UpdateReleaseProcessRequest, UpdateReleaseProcessResponse>
    {
        private readonly ReleasesStorage _releasesStorage;
        private readonly DeploymentProcessGitRepositoryFactory _deploymentProcessGitRepositoryFactory;
        private readonly IProjectContext _projectContext;
        private readonly DeploymentProcessStorage _deploymentProcessStorage;

        public UpdateReleaseProcessHandler(ReleasesStorage releasesStorage, DeploymentProcessGitRepositoryFactory deploymentProcessGitRepositoryFactory, IProjectContext projectContext, DeploymentProcessStorage deploymentProcessStorage)
        {
            _releasesStorage = releasesStorage;
            _deploymentProcessGitRepositoryFactory = deploymentProcessGitRepositoryFactory;
            _projectContext = projectContext;
            _deploymentProcessStorage = deploymentProcessStorage;
        }

        public async Task<UpdateReleaseProcessResponse> Handle(UpdateReleaseProcessRequest request)
        {
            var project = _projectContext.Project;

            var getCurrentCommitZipResult = _deploymentProcessGitRepositoryFactory.GetCurrentCommitZip(project);

            var deploymentProcessId = await _deploymentProcessStorage
                .Add(
                    project.Id,
                    getCurrentCommitZipResult.ZipFile
                );

            var release = await _releasesStorage.GetNewest(request.ProjectId);

            var newRelease = release
                .WithUpdatedCommitSha(getCurrentCommitZipResult.CreatedFromCommitSha)
                .WithUpdatedDeploymentProcess(deploymentProcessId);
            
            var releaseId = await _releasesStorage.Add(newRelease);

            return new UpdateReleaseProcessResponse
            {
                CreatedReleaseId = releaseId,
                CreatedFromCommitSha = getCurrentCommitZipResult.CreatedFromCommitSha
            };
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