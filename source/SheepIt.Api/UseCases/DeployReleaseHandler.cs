using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.CommandRunners;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.Infrastructure.Time;

namespace SheepIt.Api.UseCases
{
    public class DeployReleaseRequest : IRequest<DeployReleaseResponse>
    {
        public string ProjectId { get; set; }
        public int ReleaseId { get; set; }
        public int EnvironmentId { get; set; }
    }

    public class DeployReleaseResponse
    {
        public int CreatedDeploymentId { get; set; }
    }

    [Route("api")]
    [ApiController]
    public class DeployReleaseController : MediatorController
    {
        [HttpPost]
        [Route("deploy-release")]
        public async Task<DeployReleaseResponse> DeployRelease(DeployReleaseRequest request)
        {
            return await Handle(request);
        }
    }

    public class DeployReleaseHandler : ISyncHandler<DeployReleaseRequest, DeployReleaseResponse>
    {
        private readonly Core.Deployments.DeploymentsStorage _deploymentsStorage;
        private readonly ProjectsStorage _projectsStorage;
        private readonly ReleasesStorage _releasesStorage;
        private readonly ProcessSettings _processSettings;
        private readonly ProcessRepositoryFactory _processRepositoryFactory;
        private readonly ShellSettings _shellSettings;

        public DeployReleaseHandler(
            Core.Deployments.DeploymentsStorage deploymentsStorage,
            ProjectsStorage projectsStorage,
            ReleasesStorage releasesStorage,
            ProcessSettings processSettings,
            ProcessRepositoryFactory processRepositoryFactory,
            ShellSettings shellSettings)
        {
            _deploymentsStorage = deploymentsStorage;
            _projectsStorage = projectsStorage;
            _releasesStorage = releasesStorage;
            _processSettings = processSettings;
            _processRepositoryFactory = processRepositoryFactory;
            _shellSettings = shellSettings;
        }

        public DeployReleaseResponse Handle(DeployReleaseRequest request)
        {
            var project = _projectsStorage.Get(
                projectId: request.ProjectId
            );

            var release = _releasesStorage.Get(
                projectId: request.ProjectId,
                releaseId: request.ReleaseId
            );

            var deployment = new Deployment
            {
                ReleaseId = release.Id,
                ProjectId = request.ProjectId,
                DeployedAt = DateTime.UtcNow,
                EnvironmentId = request.EnvironmentId,
                Status = DeploymentStatus.InProgress
            };
            
            var deploymentId = _deploymentsStorage.Add(deployment);

            RunDeployment(project, release, deployment);

            return new DeployReleaseResponse
            {
                CreatedDeploymentId = deploymentId
            };
        }

        private void RunDeployment(Project project, Release release, Deployment deployment)
        {
            try
            {
                var deploymentWorkingDir = _processSettings.WorkingDir
                    .AddSegment(project.Id)
                    .AddSegment("deploying-releases")
                    .AddSegment($"{DateTime.UtcNow.FileFriendlyFormat()}_{deployment.EnvironmentId}_release-{release.Id}")
                    .ToString();

                using (var repository = _processRepositoryFactory.Clone(project.RepositoryUrl, deploymentWorkingDir))
                {
                    repository.Checkout(release.CommitSha);

                    var processOutput = new ProcessRunner(_processSettings, _shellSettings).Run(
                        processFile: repository.OpenProcessDescriptionFile(),
                        variablesForEnvironment: release.GetVariablesForEnvironment(deployment.EnvironmentId),
                        workingDir: deploymentWorkingDir
                    );

                    deployment.MarkFinished(processOutput);

                    _deploymentsStorage.Update(deployment);
                }
            }
            catch (Exception)
            {
                // todo: log exception

                deployment.MarkExecutionFailed();

                _deploymentsStorage.Update(deployment);

                throw;
            }
        }
    }
    
    public class DeployReleaseModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<DeployReleaseHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);
        }
    }
}