using System.Linq;
using System.Threading.Tasks;
using Amazon.CloudWatchLogs.Model;
using Microsoft.EntityFrameworkCore;

namespace SheepIt.Api.Core.Deployments
{
    public static class DeploymentQueryExtensions
    {
        public static async Task<Deployment> FindByIdAndProjectId(
            this IQueryable<Deployment> dbSet,
            string projectId,
            int deploymentId)
        {
            var foundDeploymentOrNull = await dbSet
                .FromProject(projectId)
                .WithId(deploymentId)
                .FirstOrDefaultAsync();

            if (foundDeploymentOrNull == null)
            {
                throw new InvalidOperationException(
                    $"Deployment with id {deploymentId} in project {projectId} could not be found."
                );
            }

            return foundDeploymentOrNull;
        }

        public static IQueryable<Deployment> WithId(
            this IQueryable<Deployment> query,
            int deploymentId)
        {
            return query.Where(deployment => deployment.Id == deploymentId);
        }
        
        public static IQueryable<Deployment> FromProject(
            this IQueryable<Deployment> query,
            string projectId)
        {
            return query.Where(deployment => deployment.ProjectId == projectId);
        }
        
        public static IQueryable<Deployment> OrderByNewest(
            this IQueryable<Deployment> query)
        {
            return query.OrderByDescending(deployment => deployment.StartedAt);
        }
    }
}