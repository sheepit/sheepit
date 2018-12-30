using System;
using System.Collections.Generic;
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
            public int EnvironmentId { get; set; }
            public string DisplayName { get; set; }
            public EnvironmentDepoloymentDto Deployment { get; set; }
        }
        
        public class EnvironmentDepoloymentDto
        {
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
            var projectEnvironments = GetProjectEnvironments(options.ProjectId);
            var deploymentInfoForEnvironments = GetDeploymentsInfoForEnvironments(options.ProjectId);

            var environments = FillEnvironmentsWithDeploymentDetails(projectEnvironments, deploymentInfoForEnvironments);

            return new ShowDashboardResponse
            {
                Environments = environments
            };
        }

        private static Domain.Environment[] GetProjectEnvironments(string projectId)
        {
            return Domain.Environments.GetAll(projectId);
        }

        private static ShowDashboardResponse.EnvironmentDto[] GetDeploymentsInfoForEnvironments(string projectId)
        {
            var environments = Domain.Deployments.GetAll(projectId)
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

            return environments;
        }

        private static ShowDashboardResponse.EnvironmentDto MapDeployment(int environmentId, Deployment deployment)
        {
            return new ShowDashboardResponse.EnvironmentDto
            {
                EnvironmentId = environmentId,
                Deployment = new ShowDashboardResponse.EnvironmentDepoloymentDto
                {
                    LastDeployedAt = deployment.DeployedAt,
                    CurrentDeploymentId = deployment.Id,
                    CurrentReleaseId = deployment.ReleaseId                    
                }
            };
        }

        private static ShowDashboardResponse.EnvironmentDto[] FillEnvironmentsWithDeploymentDetails(
            Domain.Environment[] projectEnvironments,
            ShowDashboardResponse.EnvironmentDto[] deploymentInfoForEnvironments
            )
        {
            var result = new List<ShowDashboardResponse.EnvironmentDto>();
            
            foreach (var projectEnvironment in projectEnvironments)
            {
                var deploymentInfoForEnvironment = deploymentInfoForEnvironments
                    .FirstOrDefault(x => x.EnvironmentId == projectEnvironment.Id);

                var item = new ShowDashboardResponse.EnvironmentDto
                {
                    EnvironmentId = projectEnvironment.Id,
                    DisplayName = projectEnvironment.DisplayName
                };
                
                if (deploymentInfoForEnvironment != null)
                    item.Deployment = deploymentInfoForEnvironment.Deployment;
                
                result.Add(item);
            }

            return result.ToArray();
        }
    }
}
