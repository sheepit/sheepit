using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Domain;

namespace SheepIt.Api.UseCases.Deployments
{
    public class ListDeploymentsRequest
    {
        public string ProjectId { get; set; }
        public int? ReleaseId { get; set; }
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
            public string Status { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class ListDeploymentsController : ControllerBase
    {
        [HttpPost]
        [Route("list-deployments")]
        public object ListDeployments(ListDeploymentsRequest request)
        {
            return ListDeploymentsHandler.Handle(request);
        }
    }

    public static class ListDeploymentsHandler
    {
        public static ListDeploymentResponse Handle(ListDeploymentsRequest options)
        {
            using (var database = Database.Open())
            {
                var deploymentCollection = database.GetCollection<Deployment>();

                var query = deploymentCollection
                    .Find(deployment => deployment.ProjectId == options.ProjectId);
                    
                if(options.ReleaseId.HasValue)
                    query = query.Where(x => x.ReleaseId == options.ReleaseId);
                    
                var deployments = query
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
                ReleaseId = deployment.ReleaseId,
                Status = deployment.Status.ToString()
            };
        }
    }
}