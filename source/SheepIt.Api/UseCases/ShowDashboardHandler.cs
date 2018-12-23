using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Domain;

namespace SheepIt.Api.UseCases
{
    public class ShowDashboardRequest
    {
        public string ProjectId { get; set; }
    }

    public class ShowDashboardResponse
    {
        public EnvironmentDto[] Environments { get; set; }

        public class EnvironmentDto
        {
            public string EnvironmentId { get; set; }
            public DateTime LastDeployedAt { get; set; }
            public int CurrentDeploymentId { get; set; }
            public int CurrentReleaseId { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class ShowDashboardController : ControllerBase
    {  
        [HttpPost]
        [Route("show-dashboard")]
        public object ShowDashboard(ShowDashboardRequest request)
        {
            return ShowDashboardHandler.Handle(request);
        }
    }

    public static class ShowDashboardHandler
    {
        public static ShowDashboardResponse Handle(ShowDashboardRequest options)
        {
            var environments = Deployments.GetAll(options.ProjectId)
                .GroupBy(deployment => deployment.EnvironmentId)
                .Select(grouping => MapDeployment(
                        environmentId: grouping.Key,
                        deployment: grouping
                            .OrderByDescending(deployment => deployment.DeployedAt)
                            .First()
                    )
                )
                .OrderBy(environment => environment.EnvironmentId)
                .ToArray();

            return new ShowDashboardResponse
            {
                Environments = environments
            };
        }

        private static ShowDashboardResponse.EnvironmentDto MapDeployment(string environmentId, Deployment deployment)
        {
            return new ShowDashboardResponse.EnvironmentDto
            {
                EnvironmentId = environmentId,
                LastDeployedAt = deployment.DeployedAt,
                CurrentDeploymentId = deployment.Id,
                CurrentReleaseId = deployment.ReleaseId
            };
        }
    }
}