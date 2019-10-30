using System.Linq;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Environments;

namespace SheepIt.Api.UseCases.ProjectOperations.Dashboard
{
    public static class DeploymentList
    {
        public static GetProjectDashboardResponse.DeploymentDto[] GetDeployments(Deployment[] deployments, Environment[] environments)
        {
            return deployments
                .OrderByDescending(deployment => deployment.DeployedAt)
                .Join(
                    inner: environments,
                    outerKeySelector: deployment => deployment.EnvironmentId,
                    innerKeySelector: environment => environment.Id,
                    resultSelector: MapDeployment
                )
                .ToArray();
        }

        private static GetProjectDashboardResponse.DeploymentDto MapDeployment(Deployment deployment, Environment environment)
        {
            return new GetProjectDashboardResponse.DeploymentDto
            {
                Id = deployment.Id,
                EnvironmentId = environment.Id,
                EnvironmentDisplayName = environment.DisplayName,
                DeployedAt = deployment.DeployedAt,
                PackageId = deployment.PackageId,
                Status = deployment.Status.ToString()
            };
        }
    }
}