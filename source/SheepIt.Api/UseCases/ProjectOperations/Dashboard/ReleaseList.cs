using System.Linq;
using SheepIt.Api.Core.Releases;

namespace SheepIt.Api.UseCases.ProjectOperations.Dashboard
{
    public static class ReleaseList
    {
        public static GetProjectDashboardResponse.ReleaseDto[] GetReleases(Release[] releases)
        {
            return releases
                .OrderByDescending(release => release.CreatedAt)
                .Select(MapRelease)
                .ToArray();
        }

        private static GetProjectDashboardResponse.ReleaseDto MapRelease(Release release)
        {
            return new GetProjectDashboardResponse.ReleaseDto
            {
                Id = release.Id,
                CreatedAt = release.CreatedAt
            };
        }
    }
}