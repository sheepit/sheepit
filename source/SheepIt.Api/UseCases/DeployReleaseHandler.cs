using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SheepIt.Api.CommandRunners;
using SheepIt.Api.Infrastructure;
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
        private readonly IConfiguration _configuration;
        private readonly ProcessRunner _processRunner;

        public DeployReleaseHandler(
            Domain.Deployments deployments,
            Projects projects,
            ReleasesStorage releasesStorage,
            IConfiguration configuration,
            ProcessRunner processRunner)
        {
            _deployments = deployments;
            _projects = projects;
            _releasesStorage = releasesStorage;
            _configuration = configuration;
            _processRunner = processRunner;
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
                var workingDirectoryPath = _configuration["WorkingDirectory"];
                var workingDirectory = new LocalPath(workingDirectoryPath);
                
                var deploymentWorkingDir = workingDirectory
                    .AddSegment(project.Id)
                    .AddSegment("deploying-releases")
                    .AddSegment($"{DateTime.UtcNow.FileFriendlyFormat()}_{deployment.EnvironmentId}_release-{release.Id}")
                    .ToString();

                using (var repository = ProcessRepository.Clone(project.RepositoryUrl, deploymentWorkingDir))
                {
                    repository.Checkout(release.CommitSha);

                    var processOutput = _processRunner.Run(
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