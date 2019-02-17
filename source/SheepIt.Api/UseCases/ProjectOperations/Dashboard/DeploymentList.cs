using System.Linq;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Environments;

namespace SheepIt.Api.UseCases.ProjectOperations.Dashboard
{
    public static class DeploymentList
    {
        public static GetDashboardResponse.DeploymentDto[] GetDeployments(Deployment[] deployments, Environment[] environments)
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

        private static GetDashboardResponse.DeploymentDto MapDeployment(Deployment deployment, Environment environment)
        {
            return new GetDashboardResponse.DeploymentDto
            {
                Id = deployment.Id,
                EnvironmentId = environment.Id,
                EnvironmentDisplayName = environment.DisplayName,
                DeployedAt = deployment.DeployedAt,
                ReleaseId = deployment.ReleaseId,
                Status = deployment.Status.ToString()
            };
        }
    }
}