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
        [HttpPost]
        [Route("deploy-release")]
        public object DeployRelease(DeployReleaseRequest request)
        {
            return DeployReleaseHandler.Handle(request);
        }
    }

    public static class DeployReleaseHandler
    {
        public static DeployReleaseResponse Handle(DeployReleaseRequest request)
        {
            var project = Projects.Get(
                projectId: request.ProjectId
            );

            var release = ReleasesStorage.Get(
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
            
            var deploymentId = Domain.Deployments.Add(deployment);

            RunDeployment(project, release, deployment);

            return new DeployReleaseResponse
            {
                CreatedDeploymentId = deploymentId
            };
        }

        private static void RunDeployment(Project project, Release release, Deployment deployment)
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
                        variablesForEnvironment: release.GetVariablesForEnvironment("dev"), // variablesForEnvironment: release.GetVariablesForEnvironment(deployment.EnvironmentId), TODO: hardcoded environment
                        workingDir: deploymentWorkingDir
                    );

                    deployment.MarkFinished(processOutput);

                    Domain.Deployments.Update(deployment);
                }
            }
            catch (Exception)
            {
                // todo: log exception

                deployment.MarkExecutionFailed();

                Domain.Deployments.Update(deployment);

                throw;
            }
        }
    }
}