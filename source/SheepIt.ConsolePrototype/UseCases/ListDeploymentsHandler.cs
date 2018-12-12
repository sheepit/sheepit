using System;
using System.Linq;
using SheepIt.Domain;

namespace SheepIt.ConsolePrototype.UseCases
{
    public class ListDeploymentsRequest
    {
        public string ProjectId { get; set; }
    }

    public class ListDeploymentResponse
    {
        public DeploymentDto[] Deployments { get; set; }

        public class DeploymentDto
        {
            public int Id { get; set; }
            public int ReleaseId { get; set; }
            public DateTime DeployedAt { get; set; }
            public string EnvironmentId { get; set; }
        }
    }

    public static class ListDeploymentsHandler
    {
        public static ListDeploymentResponse Handle(ListDeploymentsRequest options)
        {
            using (var database = Database.Open())
            {
                var deploymentCollection = database.GetCollection<Deployment>();

                var deployments = deploymentCollection
                    .Find(deployment => deployment.ProjectIt == options.ProjectId)
                    .OrderBy(deployment => deployment.DeployedAt)
                    .Select(Map)
                    .ToArray();

                return new ListDeploymentResponse
                {
                    Deployments = deployments
                };
            }
        }

        private static ListDeploymentResponse.DeploymentDto Map(Deployment deployment)
        {
            return new ListDeploymentResponse.DeploymentDto
            {
                Id = deployment.Id,
                EnvironmentId = deployment.EnvironmentId,
                DeployedAt = deployment.DeployedAt,
                ReleaseId = deployment.ReleaseId
            };
        }
    }
}