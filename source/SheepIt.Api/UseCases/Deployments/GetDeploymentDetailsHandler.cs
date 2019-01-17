using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Domain;

namespace SheepIt.Api.UseCases.Deployments
{
    public class GetDeploymentDetailsRequest
    {
        public string ProjectId { get; set; }
        public int DeploymentId { get; set; }
    }

    public class GetDeploymentDetailsResponse
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public int ReleaseId { get; set; }
        public int EnvironmentId { get; set; }
        public string EnvironmentDisplayName { get; set; }
        public DateTime DeployedAt { get; set; }
        public CommandOutput[] StepResults { get; set; }

        public class CommandOutput
        {
            public string Command { get; set; }
            public bool Successful { get; set; }
            public string[] Output { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class GetDeploymentDetailsController : ControllerBase
    {
        private readonly GetDeploymentDetailsHandler _handler;

        public GetDeploymentDetailsController(GetDeploymentDetailsHandler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        [Route("get-deployment-details")]
        public object GetDeploymentDetails(GetDeploymentDetailsRequest request)
        {
            return _handler.Handle(request);
        }
    }

    public class GetDeploymentDetailsHandler
    {
        private readonly Domain.Deployments _deployments = new Domain.Deployments();
        private readonly Domain.Environments _environments = new Domain.Environments();
        private readonly Projects _projects = new Projects();
        private readonly ReleasesStorage _releasesStorage = new ReleasesStorage();

        public GetDeploymentDetailsResponse Handle(GetDeploymentDetailsRequest request)
        {
            var project = _projects.Get(
                projectId: request.ProjectId
            );

            var deployment = _deployments.Get(
                projectId: request.ProjectId,
                deploymentId: request.DeploymentId
            );

            var environment = _environments.Get(
                environmentId: deployment.EnvironmentId);

            var release = _releasesStorage.Get(
                projectId: request.ProjectId,
                releaseId: deployment.ReleaseId
            );

            return new GetDeploymentDetailsResponse
            {
                Id = deployment.Id,
                Status = deployment.Status.ToString(),
                ReleaseId = deployment.ReleaseId,
                EnvironmentId = environment.Id,
                EnvironmentDisplayName = environment.DisplayName,
                DeployedAt = deployment.DeployedAt,
                StepResults = GetStepResults(deployment)
            };
        }

        private GetDeploymentDetailsResponse.CommandOutput[] GetStepResults(Deployment deployment)
        {
            // todo: this is ugly af
            var steps = deployment.ProcessOutput?.Steps ?? Enumerable.Empty<ProcessStepResult>();

            return steps
                .Select(MapCommandOutput)
                .ToArray();
        }

        private GetDeploymentDetailsResponse.CommandOutput MapCommandOutput(ProcessStepResult result)
        {
            return new GetDeploymentDetailsResponse.CommandOutput
            {
                Command = result.Command,
                Successful = result.Successful,
                Output = result.Output.ToArray()
            };
        }
    }
}