using System.Collections.Generic;
using System.Linq;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Core.Packages;

namespace SheepIt.Api.UseCases.ProjectOperations.Dashboard
{
    public static class EnvironmentList
    {
        public static GetProjectDashboardResponse.EnvironmentDto[] GetEnvironments(
            Environment[] environments,
            Deployment[] deployments,
            Package[] packages)
        {
            return environments
                .Select(environment =>
                {
                    var lastDeploymentOrNull = deployments
                        .Where(deployment => deployment.EnvironmentId == environment.Id)
                        .OrderByDescending(deployment => deployment.DeployedAt)
                        .FirstOrDefault();

                    return MapEnvironment(environment, lastDeploymentOrNull, packages);
                })
                .ToArray();
        }

        private static GetProjectDashboardResponse.EnvironmentDto MapEnvironment(
            Environment environment,
            Deployment lastDeploymentOrNull,
            Package[] packages)
        {
            return new GetProjectDashboardResponse.EnvironmentDto
            {
                EnvironmentId = environment.Id,
                DisplayName = environment.DisplayName,
                Deployment = MapLastDeployment(lastDeploymentOrNull, packages)
            };
        }

        private static GetProjectDashboardResponse.EnvironmentDeploymentDto MapLastDeployment(
            Deployment lastDeploymentOrNull, Package[] packages)
        {
            if (lastDeploymentOrNull != null)
            {
                var packageDescription =
                    packages
                        .FirstOrDefault(x => x.Id == lastDeploymentOrNull.PackageId)
                        ?.Description ?? string.Empty;

                return new GetProjectDashboardResponse.EnvironmentDeploymentDto
                {
                    CurrentDeploymentId = lastDeploymentOrNull.Id,
                    CurrentPackageId = lastDeploymentOrNull.PackageId,
                    CurrentPackageDescription = packageDescription, 
                    LastDeployedAt = lastDeploymentOrNull.DeployedAt
                };
            }

            return null;
        }
    }
}