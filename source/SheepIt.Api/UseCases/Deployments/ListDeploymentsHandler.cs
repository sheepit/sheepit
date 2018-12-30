using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Domain;
using Environment = SheepIt.Domain.Environment;

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
            public int EnvironmentId { get; set; }
            public string EnvironmentDisplayName { get; set; }
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
            var deployments = GetDeployments(options).ToArray();
            var environments = GetEnvironments(options.ProjectId);

            var deploymentsData = MergeData(deployments, environments);

            return new ListDeploymentResponse
            {
                Deployments = deploymentsData.ToArray()
            };
        }

        private static IEnumerable<Deployment> GetDeployments(ListDeploymentsRequest options)
        {
            using (var database = Database.Open())
            {
                var deploymentCollection = database.GetCollection<Deployment>();

                var query = deploymentCollection
                    .Find(deployment => deployment.ProjectId == options.ProjectId);

                if (options.ReleaseId.HasValue)
                    query = query.Where(x => x.ReleaseId == options.ReleaseId);

                var deployments = query
                    .OrderBy(deployment => deployment.DeployedAt);

                return deployments;
            }
        }

        private static Environment[] GetEnvironments(string projectId)
        {
            return Domain.Environments.GetAll(projectId);
        }

        private static List<ListDeploymentResponse.DeploymentDto> MergeData(
            Deployment[] deployments, Environment[] environments)
        {
            var result = new List<ListDeploymentResponse.DeploymentDto>();
            
            foreach (var deployment in deployments)
            {
                var environment = environments.Single(x => x.Id == deployment.EnvironmentId);
                
                result.Add(new ListDeploymentResponse.DeploymentDto
                {
                    Id = deployment.Id,
                    EnvironmentId = environment.Id,
                    EnvironmentDisplayName = environment.DisplayName,
                    DeployedAt = deployment.DeployedAt,
                    ReleaseId = deployment.ReleaseId,
                    Status = deployment.Status.ToString()
                });
            }

            return result;
        }
    }
}
