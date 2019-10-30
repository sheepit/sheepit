using System.Linq;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Environments;

namespace SheepIt.Api.UseCases.ProjectOperations.Dashboard
{
    public static class EnvironmentList
    {
        public static GetProjectDashboardResponse.EnvironmentDto[] GetEnvironments(
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

        private static GetProjectDashboardResponse.EnvironmentDto MapEnvironment(
            Environment environment,
            Deployment lastDeploymentOrNull)
        {
            return new GetProjectDashboardResponse.EnvironmentDto
            {
                EnvironmentId = environment.Id,
                DisplayName = environment.DisplayName,
                Deployment = MapLastDeployment(lastDeploymentOrNull)
            };
        }

        private static GetProjectDashboardResponse.EnvironmentDeploymentDto MapLastDeployment(Deployment lastDeploymentOrNull)
        {
            if (lastDeploymentOrNull != null)
            {
                return new GetProjectDashboardResponse.EnvironmentDeploymentDto
                {
                    CurrentDeploymentId = lastDeploymentOrNull.Id,
                    CurrentPackageId = lastDeploymentOrNull.PackageId,
                    LastDeployedAt = lastDeploymentOrNull.DeployedAt
                };
            }

            return null;
        }
    }
}