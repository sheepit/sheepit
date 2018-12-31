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
        [HttpPost]
        [Route("get-deployment-details")]
        public object GetDeploymentDetails(GetDeploymentDetailsRequest request)
        {
            return GetDeploymentDetailsHandler.Handle(request);
        }
    }

    public static class GetDeploymentDetailsHandler
    {
        public static GetDeploymentDetailsResponse Handle(GetDeploymentDetailsRequest request)
        {
            var project = Projects.Get(
                projectId: request.ProjectId
            );

            var deployment = Domain.Deployments.Get(
                projectId: request.ProjectId,
                deploymentId: request.DeploymentId
            );

            var environment = Domain.Environments.Get(
                environmentId: deployment.EnvironmentId);
            
            var release = ReleasesStorage.Get(
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

        private static GetDeploymentDetailsResponse.CommandOutput[] GetStepResults(Deployment deployment)
        {
            // todo: this is ugly af
            var steps = deployment.ProcessOutput?.Steps ?? Enumerable.Empty<ProcessStepResult>();
            
            return steps
                .Select(MapCommandOutput)
                .ToArray();
        }

        private static GetDeploymentDetailsResponse.CommandOutput MapCommandOutput(ProcessStepResult result)
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