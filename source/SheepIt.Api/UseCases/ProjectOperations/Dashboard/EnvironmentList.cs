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
                        .OrderByDescending(deployment => deployment.StartedAt)
                        .FirstOrDefault();

                    return MapEnvironment(environment, lastDeploymentOrNull, packages);
                })
                .ToArray();
        }

        private static GetProjectDashboardResponse.EnvironmentDto MapEnvironment(
            Environment environment,
            Deployment lastDeploymentMongoEntityOrNull,
            Package[] packages)
        {
            return new GetProjectDashboardResponse.EnvironmentDto
            {
                EnvironmentId = environment.Id,
                DisplayName = environment.DisplayName,
                Deployment = MapLastDeployment(lastDeploymentMongoEntityOrNull, packages)
            };
        }

        private static GetProjectDashboardResponse.EnvironmentDeploymentDto MapLastDeployment(
            Deployment lastDeploymentMongoEntityOrNull, 
            Package[] packages)
        {
            if (lastDeploymentMongoEntityOrNull != null)
            {
                var packageDescription =
                    packages
                        .FirstOrDefault(x => x.Id == lastDeploymentMongoEntityOrNull.PackageId)
                        ?.Description ?? string.Empty;

                return new GetProjectDashboardResponse.EnvironmentDeploymentDto
                {
                    CurrentDeploymentId = lastDeploymentMongoEntityOrNull.Id,
                    CurrentPackageId = lastDeploymentMongoEntityOrNull.PackageId,
                    CurrentPackageDescription = packageDescription, 
                    LastDeployedAt = lastDeploymentMongoEntityOrNull.StartedAt
                };
            }

            return null;
        }
    }
}