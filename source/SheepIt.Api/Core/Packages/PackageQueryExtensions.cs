using System.Linq;
using System.Threading.Tasks;
using Amazon.CloudWatchLogs.Model;
using Microsoft.EntityFrameworkCore;

namespace SheepIt.Api.Core.Packages
{
    public static class PackageQueryExtensions
    {
        public static async Task<Package> FindNewestInProject(
            this IQueryable<Package> query,
            string projectId)
        {
            return await query
                .FromProject(projectId)
                .OrderByNewest()
                .FirstAsync();
        }
        
        public static async Task<Package> FindByIdAndProjectId(
            this IQueryable<Package> query,
            int packageId,
            string projectId)
        {
            var foundPackageOrNull = await query
                .WithId(packageId)
                .FromProject(projectId)
                .SingleOrDefaultAsync();

            if (foundPackageOrNull == null)
            {
                throw new InvalidOperationException(
                    $"Package with id {packageId} in project {projectId} could not be found."
                );
            }

            return foundPackageOrNull;
        }
        
        public static IQueryable<Package> WithId(
            this IQueryable<Package> query,
            int packageId)
        {
            return query.Where(package => package.Id == packageId);
        }
        
        public static IQueryable<Package> FromProject(
            this IQueryable<Package> query,
            string projectId)
        {
            return query.Where(package => package.ProjectId == projectId);
        }
        
        public static IQueryable<Package> OrderByNewest(
            this IQueryable<Package> query)
        {
            return query.OrderByDescending(package => package.CreatedAt);
        }
    }
}