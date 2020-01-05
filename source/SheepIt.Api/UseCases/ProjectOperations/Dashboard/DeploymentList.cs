using System.Linq;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Core.Packages;

namespace SheepIt.Api.UseCases.ProjectOperations.Dashboard
{
    public static class DeploymentList
    {
        public static GetProjectDashboardResponse.DeploymentDto[] GetDeployments(
            Deployment[] deployments,
            Environment[] environments,
            PackageMongoEntity[] packages
            )
        {
            return deployments
                .OrderByDescending(deployment => deployment.DeployedAt)
                .Join(
                    inner: environments,
                    outerKeySelector: deployment => deployment.EnvironmentId,
                    innerKeySelector: environment => environment.Id,
                    resultSelector: (deployment, environment) =>
                        MapDeployment(deployment, environment, packages)
                )
                .ToArray();
        }

        private static GetProjectDashboardResponse.DeploymentDto MapDeployment(
            Deployment deployment,
            Environment environment,
            PackageMongoEntity[] packages)
        {
            var package = packages.FirstOrDefault(x => x.Id == deployment.PackageId);
            var description = package?.Description ?? string.Empty;
            
            return new GetProjectDashboardResponse.DeploymentDto
            {
                Id = deployment.Id,
                EnvironmentId = environment.Id,
                EnvironmentDisplayName = environment.DisplayName,
                DeployedAt = deployment.DeployedAt,
                PackageId = deployment.PackageId,
                PackageDescription = description,
                Status = deployment.Status.ToString()
            };
        }
    }
}