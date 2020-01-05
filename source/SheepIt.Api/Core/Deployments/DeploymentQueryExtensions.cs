using System.Linq;
using System.Threading.Tasks;
using Amazon.CloudWatchLogs.Model;
using Microsoft.EntityFrameworkCore;

namespace SheepIt.Api.Core.Deployments
{
    public static class DeploymentQueryExtensions
    {
        public static async Task<Deployment> FindByIdAndProjectId(
            this DbSet<Deployment> dbSet,
            string projectId,
            int deploymentId)
        {
            var foundDeploymentOrNull = await dbSet
                .Where(deployment => deployment.ProjectId == projectId)
                .Where(deployment => deployment.Id == deploymentId)
                .FirstOrDefaultAsync();

            if (foundDeploymentOrNull == null)
            {
                throw new InvalidOperationException(
                    $"Deployment with id {deploymentId} in project {projectId} could not be found."
                );
            }

            return foundDeploymentOrNull;
        }

    }
}