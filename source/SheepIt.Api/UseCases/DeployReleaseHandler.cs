using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.CommandRunners;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Domain;
using SheepIt.Utils.Extensions;

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
        private readonly Domain.Deployments _deployments;
        private readonly Projects _projects;
        private readonly ReleasesStorage _releasesStorage;
        private readonly ProcessSettings _processSettings;
        private readonly ProcessRepositoryFactory _processRepositoryFactory;

        public DeployReleaseHandler(Domain.Deployments deployments, Projects projects, ReleasesStorage releasesStorage, ProcessSettings processSettings, ProcessRepositoryFactory processRepositoryFactory)
        {
            _deployments = deployments;
            _projects = projects;
            _releasesStorage = releasesStorage;
            _processSettings = processSettings;
            _processRepositoryFactory = processRepositoryFactory;
        }

        public DeployReleaseResponse Handle(DeployReleaseRequest request)
        {
            var project = _projects.Get(
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
            
            var deploymentId = _deployments.Add(deployment);

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

                    var processOutput = new ProcessRunner().Run(
                        processFile: repository.OpenProcessDescriptionFile(),
                        variablesForEnvironment: release.GetVariablesForEnvironment(deployment.EnvironmentId),
                        workingDir: deploymentWorkingDir
                    );

                    deployment.MarkFinished(processOutput);

                    _deployments.Update(deployment);
                }
            }
            catch (Exception)
            {
                // todo: log exception

                deployment.MarkExecutionFailed();

                _deployments.Update(deployment);

                throw;
            }
        }
    }
}