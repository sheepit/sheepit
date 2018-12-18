using System;
using System.Linq;
using SheepIt.Domain;

namespace SheepIt.ConsolePrototype.UseCases
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
        public string EnvironmentId { get; set; }
        public DateTime DeployedAt { get; set; }
        public CommandOutput[] StepResults { get; set; }

        public class CommandOutput
        {
            public string Command { get; set; }
            public bool Successful { get; set; }
            public string[] Output { get; set; }
        }
    }

    public static class GetDeploymentDetailsHandler
    {
        public static GetDeploymentDetailsResponse Handle(GetDeploymentDetailsRequest request)
        {
            var project = Projects.Get(
                projectId: request.ProjectId
            );

            var deployment = Deployments.Get(
                projectId: request.ProjectId,
                deploymentId: request.DeploymentId
            );

            var release = ReleasesStorage.Get(
                projectId: request.ProjectId,
                releaseId: deployment.Id
            );
            
            return new GetDeploymentDetailsResponse
            {
                Id = deployment.Id,
                Status = deployment.Status.ToString(),
                ReleaseId = deployment.ReleaseId,
                EnvironmentId = deployment.EnvironmentId,
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