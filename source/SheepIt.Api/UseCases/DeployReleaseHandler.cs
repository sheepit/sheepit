using System;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.CommandRunners;
using SheepIt.Api.Infrastructure;
using SheepIt.Domain;
using SheepIt.Utils.Extensions;

namespace SheepIt.Api.UseCases
{
    public class DeployReleaseRequest
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
    public class DeployReleaseController : ControllerBase
    {
        private readonly DeployReleaseHandler _handler;

        public DeployReleaseController(DeployReleaseHandler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        [Route("deploy-release")]
        public object DeployRelease(DeployReleaseRequest request)
        {
            return _handler.Handle(request);
        }
    }

    public class DeployReleaseHandler
    {
        private readonly Domain.Deployments _deployments = new Domain.Deployments();
        private readonly Projects _projects = new Projects();
        private readonly ReleasesStorage _releasesStorage = new ReleasesStorage();
        
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
                var deploymentWorkingDir = Settings.WorkingDir
                    .AddSegment(project.Id)
                    .AddSegment("deploying-releases")
                    .AddSegment($"{DateTime.UtcNow.FileFriendlyFormat()}_{deployment.EnvironmentId}_release-{release.Id}")
                    .ToString();

                using (var repository = ProcessRepository.Clone(project.RepositoryUrl, deploymentWorkingDir))
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