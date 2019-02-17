using System.Linq;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Environments;

namespace SheepIt.Api.UseCases.ProjectOperations.Dashboard
{
    public static class EnvironmentList
    {
        public static GetDashboardResponse.EnvironmentDto[] GetEnvironments(
            Environment[] environments,
            Deployment[] deployments)
        {
            return environments
                .Select(environment =>
                {
                    var lastDeploymentOrNull = deployments
                        .Where(deployment => deployment.EnvironmentId == environment.Id)
                        .OrderByDescending(deployment => deployment.DeployedAt)
                        .FirstOrDefault();

                    return MapEnvironment(environment, lastDeploymentOrNull);
                })
                .ToArray();
        }

        private static GetDashboardResponse.EnvironmentDto MapEnvironment(
            Environment environment,
            Deployment lastDeploymentOrNull)
        {
            return new GetDashboardResponse.EnvironmentDto
            {
                EnvironmentId = environment.Id,
                DisplayName = environment.DisplayName,
                Deployment = MapLastDeployment(lastDeploymentOrNull)
            };
        }

        private static GetDashboardResponse.EnvironmentDeploymentDto MapLastDeployment(Deployment lastDeploymentOrNull)
        {
            if (lastDeploymentOrNull != null)
            {
                return new GetDashboardResponse.EnvironmentDeploymentDto
                {
                    CurrentDeploymentId = lastDeploymentOrNull.Id,
                    CurrentReleaseId = lastDeploymentOrNull.ReleaseId,
                    LastDeployedAt = lastDeploymentOrNull.DeployedAt
                };
            }

            return null;
        }
    }
}