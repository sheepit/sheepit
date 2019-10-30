using System.Linq;
using SheepIt.Api.Core.Packages;

namespace SheepIt.Api.UseCases.ProjectOperations.Dashboard
{
    public static class PackageList
    {
        public static GetProjectDashboardResponse.PackageDto[] GetPackages(Package[] packages)
        {
            return packages
                .OrderByDescending(package => package.CreatedAt)
                .Select(MapPackage)
                .ToArray();
        }

        private static GetProjectDashboardResponse.PackageDto MapPackage(Package package)
        {
            return new GetProjectDashboardResponse.PackageDto
            {
                Id = package.Id,
                CreatedAt = package.CreatedAt
            };
        }
    }
}