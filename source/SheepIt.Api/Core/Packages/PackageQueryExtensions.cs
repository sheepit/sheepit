using System.Linq;
using System.Threading.Tasks;
using Amazon.CloudWatchLogs.Model;
using Microsoft.EntityFrameworkCore;

namespace SheepIt.Api.Core.Packages
{
    public static class PackageQueryExtensions
    {
        public static async Task<Package> FindByIdAndProjectId(this DbSet<Package> dbSet, int packageId, string projectId)
        {
            var foundPackageOrNull = await dbSet
                .Where(package => package.Id == packageId)
                .Where(package => package.ProjectId == projectId)
                .SingleOrDefaultAsync();

            if (foundPackageOrNull == null)
            {
                throw new InvalidOperationException(
                    $"Package with id {packageId} in project {projectId} could not be found.");
            }

            return foundPackageOrNull;
        }
        
    }
}